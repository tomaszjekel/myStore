using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyStore.Domain;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;
using MyStore.Services.DTO;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UnityEngine;
using Stripe;
using Stripe.FinancialConnections;

namespace MyStore.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IFileService _fileService;

        public ProductsController(IProductService productService, IFileService fileService)
        {
            _productService = productService;
            _fileService = fileService;
        }

        [HttpGet("browse")]
        public async Task<IActionResult> Browse(string keyword, int? pageIndex, Guid userId)
        {
            ViewBag.Shop = "Shop";
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var products = await _productService.BrowseByUserId(keyword, pageIndex, userId);


            var viewModels = products.Select(p =>
                new ProductViewModel
                {
                    Id = p.Id,
                    UserId = userGuid,
                    ProductUserId = p.UserId,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    Description = p.Description,
                    Files = p.Files
                });
            //if (userGuid != Guid.Empty && name !="all")
            //    viewModels = viewModels.Where(c => c.ProductUserId == userGuid);

            ProductNewViewModel newModel = new ProductNewViewModel();
            newModel.Products = viewModels.ToList();
            newModel.HasNextPage = products.HasNextPage;
            newModel.HasPreviousPage = products.HasPreviousPage;
            newModel.PageIndex = products.PageIndex;
            newModel.TotalPages = products.TotalPages;

            return View(newModel);
        }

        [HttpGet("browseAll")]
        public async Task<IActionResult> BrowseAll(string name)
        {
            var products = await _productService.BrowseAsync(name);
            var viewModels = products.Select(p =>
                new ProductViewModel
                {
                    Id = p.Id,
                    ProductUserId = p.UserId,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    Description = p.Description
                });



            return View(viewModels);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var fileList = await _fileService.BrowseByProductAsync(product.UserId, product.Id);
            var cities = await _productService.GetCities();
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Files = product.Files,
                Description = product.Description,
                City = cities.Where(x => x.Id == product.CityId).Select(x => x.Name).FirstOrDefault()
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet("create")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create()
        {


            //  QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //System.Random rnd = new System.Random();

            //QRCodeData qrCodeData = new QRCodeGenerator().CreateQrCode("your nr->" + rnd.Next(0, 100), QRCodeGenerator.ECCLevel.Q);
            //Bitmap qrCodeImage = new QRCode(qrCodeData).GetGraphic(20, "#ffffff", "#4d004d");
            //using (var qr = new FileStream(location + DateTime.Now.Millisecond + ".jpg", FileMode.Create))
            //{
            //    qrCodeImage.Save(qr, ImageFormat.Jpeg);
            //}


            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var fileList = await _fileService.BrowseAsync(userGuid);
            var cities = await _productService.GetCities();
            var citiy = cities.Select(x => new { Value = x.Id, Text = x.Name });
            SelectList list = new SelectList(citiy, "Value", "Text");

            var viewModel = new CreateProductViewModel() { Files = fileList.ToList(), Cities = list };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return await Create();
            }

            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var newId = Guid.NewGuid();
            await _productService.CreateAsync(newId, userId, viewModel.Name,
            viewModel.Category, viewModel.Price, viewModel.Description, Int32.Parse(viewModel.SelectedCity ?? "0"), viewModel.Files.FirstOrDefault().Name);

            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //wwait _fileService.UpdateAsync(newId);
            return RedirectToAction("browse", new { userId = userId });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BrowseProducts query)
        {
            var products = await _productService.BrowseAsync(query.Name);

            return Ok(products);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    var product = await _productService.GetAsync(id);
        //    if (product != null)
        //    {
        //        return Ok(product);
        //    }

        //    return NotFound();
        //}

        [Authorize]
        [HttpGet("Edit/")]
        public async Task<IActionResult> Edit(Guid productId)
        {
            var product = await _productService.GetAsync(productId);
            if (product != null)
            {
                var cities = await _productService.GetCities();
                var citiy = cities.Select(x => new { Value = x.Id, Text = x.Name });
                SelectList list = new SelectList(citiy, "Value", "Text");
                EditProductViewModel edit = new EditProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Files = product.Files.Select(x => new FileDto { Id = x.Id, Name = x.Name, ProductId = x.ProductId }).ToList(),
                    Category = product.Category,
                    Cities = list
                };
                if (product.CityId != 0)
                {
                    var selected = list.Where(x => x.Value == product.CityId.ToString()).First();
                    selected.Selected = true;
                }


                return View(edit);
            }

            return NotFound();
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(EditProductViewModel editModel)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                return await Edit(editModel.Id);
            }
            if (editModel != null)
            {
                Domain.Product p = new Domain.Product
                {
                    Id = editModel.Id,
                    Name = editModel.Name,
                    Category = editModel.Category,
                    CityId = Int32.Parse(editModel.SelectedCity ?? "0"),
                    Description = editModel.Description,
                    Price = editModel.Price
                };
                await _productService.UpdateProduct(p);

                //return View(edit);
                //return RedirectToAction(editModel.Id.ToString(), "Products");
                return RedirectToAction("browse", new { userId = userId });
            }

            return NotFound();
        }

        [Authorize]
        [HttpGet("/Delete/{imageId}/{productId}")]
        public async Task<IActionResult> Delete(Guid imageId, Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteImageFromProduct(productId, imageId, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
            return RedirectToAction("edit", "Products", new { productId = productId });
            //return await Edit(productId);

        }

        [Authorize]
        [HttpGet("/DeleteImage/{imageName}")]
        public async Task<IActionResult> DeleteImage(string imageName, Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteImage(imageName, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
            return RedirectToAction("create");

        }

        [Authorize]
        [HttpGet("/DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteProduct(productId, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
            return RedirectToAction("browse", new { userId = userId });
        }

        [Authorize]
        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> Post([FromBody] CreateProduct request)
        {
            //await _productService.CreateAsync(Guid.NewGuid(), request.UserId, request.Name,
            //    request.Category, request.Price, request.Description,(Int32.Parse("1")));

            return Ok();
        }

        [Authorize]
        [HttpPost("Upload/{action_name}/{productId}")]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files, string action_name, Guid productId)
        {
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);

            if (action_name == "create")
            {
                var fileList = await _productService.UploadandResize(files, userGuid, productId);
                // return RedirectToAction("create");
                return Json(new { files = fileList });

            }
            else if (action_name == "edit")
            {
                var fileList = await _productService.UploadandResize(files, userGuid, productId);
                // return RedirectToAction("create");
                return Json(new { files = fileList });
            }

            return RedirectToAction(productId.ToString(), "Products");

            // _productService.UploadandResize(files, userGuid, productId );
            //return RedirectToAction(productId.ToString(), "Products");




        }



        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession(string amount)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
    "card","p24"
  },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
        {
         new Stripe.Checkout.SessionLineItemOptions
         {
             PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
             {
                 UnitAmount=Convert.ToInt32(2)*100,
                 Currency="pln",
                 ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                 {
                     Name = "T-shert"
                 },
             },
             Quantity=2,
         },
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:5000/Home/Success",
                CancelUrl = "http://localhost:5000/Home/cancel",
            };
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
    }
    public class CreateProduct
    {
        [Required]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }

    public class BrowseProducts
    {
        public string Name { get; set; }
    }
}