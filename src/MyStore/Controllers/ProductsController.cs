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
using MyStore.Infrastructure.EF;
using System.Xml.Linq;

namespace MyStore.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IFileService _fileService;
        private readonly MyStoreContext _context;

        public ProductsController(IProductService productService, IFileService fileService, MyStoreContext context)
        {
            _productService = productService;
            _fileService = fileService;
            _context = context;
        }

        [HttpGet("browse")]
        public async Task<IActionResult> Browse(string keyword, int? pageIndex, Guid userId, Guid? category)
        {
            ViewBag.Shop = "Shop";
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var products = await _productService.BrowseByUserId(keyword, pageIndex, userId, category);


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
                    Files = p.Files,
                    Variants = p.Variants,
                });
            //if (userGuid != Guid.Empty && name !="all")
            //    viewModels = viewModels.Where(c => c.ProductUserId == userGuid);

            ProductNewViewModel newModel = new ProductNewViewModel();
            newModel.Catgories = _context.Categories.ToList();

            newModel.Category = category;
            if(category != null)
            {
                newModel.Count = _context.Products.Where(x=>x.Category == category.ToString() && x.Deleted == false).Count();
            }
            else
            {
                newModel.Count = _context.Products.Where(x=>x.Deleted== false).Count();
            }
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
            
            List<Domain.Size> namesSize = new List<Domain.Size>();
            foreach(var n in product.Variants)
            {
                var name = _context.Sizes.Where(x => x.Id == n.SizeId).FirstOrDefault().Name;
                var value = _context.Sizes.Where(y => y.Id == n.SizeId).FirstOrDefault().Id;
                namesSize.Add(new Domain.Size() { Id = value , Name=name});
            }
            
            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Files = product.Files,
                Description = product.Description,
                City = cities.Where(x => x.Id == product.CityId).Select(x => x.Name).FirstOrDefault(),
                Variants= product.Variants,
                NamesSize= namesSize
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

            var categories = _context.Categories.ToList();
            var cat = categories.Select(x => new { Value = x.Id, Text = x.Name });
            SelectList listCategories = new SelectList(cat, "Value", "Text");

            var colors = _context.Colors.ToList();
            var col = colors.Select(x => new { Value = x.Id, Text = x.Name });
            SelectList listColors = new SelectList(col, "Value", "Text");
            var listColors1 = listColors.ToList();
            listColors1.Insert(0, new SelectListItem()
            {
                Value = null, // <=========== Here lies the problem...
                Text = "None"
            });


            var sizes = _context.Sizes.ToList();
            var siz = sizes.Select(x => new { Value = x.Id, Text = x.Name });
            SelectList listSizes = new SelectList(siz, "Value", "Text");
            var listSizes1 = listSizes.ToList();
            listSizes1.Insert(0, new SelectListItem()
            {
                Value = null, // <=========== Here lies the problem...
                Text = "None"
            });

            var variants = _context.ProductVariants.Where(x => x.ProductId == null && x.UserId == userGuid).ToList();

            var viewModel = new CreateProductViewModel() 
            { 
                Files = fileList.ToList(),
                Cities = list,
                Categories = listCategories,
                Sizes = listSizes1,
                Colors= listColors1,
                Variants= variants,
            };

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

            var variants = _context.ProductVariants.Where(x=>x.ProductId == null && x.UserId == userId).ToList();

            Domain.Product p = new Domain.Product 
            { 
                Id = newId,
                UserId = userId,
                Name = viewModel.Name,
                Category = viewModel.Category,
                Price = viewModel.Price,
                Description = viewModel.Description, 
                CityId = Int32.Parse(viewModel.SelectedCity ?? "0"), 
                Img = "",
                Deleted = false,
                Variants = variants,
                
            };
            await _productService.CreateAsync(p);

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
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(Guid productId)
        {
            var product = await _productService.GetAsync(productId);
            if (product != null)
            {
                var cities = await _productService.GetCities();
                var citiy = cities.Select(x => new { Value = x.Id, Text = x.Name });
                SelectList list = new SelectList(citiy, "Value", "Text");

                var categories = _context.Categories.ToList();
                var cat = categories.Select(x => new { Value = x.Id, Text = x.Name });
                SelectList listCategories = new SelectList(cat, "Value", "Text");

                EditProductViewModel edit = new EditProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Files = product.Files.Select(x => new FileDto { Id = x.Id, Name = x.Name, ProductId = x.ProductId }).ToList(),
                    Category = product.Category,
                    Categories = listCategories.ToList(),
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

        [HttpPost("Edit")]
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
                return RedirectToAction("EditProduct", "Admin");
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
            //return RedirectToAction("browse", new { userId = userId });
            return RedirectToAction("EditProduct", "Admin");
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

        [Authorize]
        [HttpPost("/Upload/{action_name}/{productId}")]
        public async Task<IActionResult> Upload1(ICollection<IFormFile> files, string action_name, Guid productId)
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
    "card","blik"
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
                     Name = "T-shert",
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


        [HttpPost("create-checkout-session1")]
        public ActionResult CreateCheckoutSession1(string amount)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
    "card","blik"
  },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
        {
         
        },
                Mode = "payment",
                SuccessUrl = "http://localhost:5000/Home/Success",
                CancelUrl = "http://localhost:5000/Home/cancel",
            };

            var currentLineItem = new Stripe.Checkout.SessionLineItemOptions
            {
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = Convert.ToInt32(2) * 100,
                    Currency = "pln",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "T-shert",
                    },
                },
                Quantity = 2,
            };
            options.LineItems.Add(currentLineItem);
            options.LineItems.Add(currentLineItem);

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        [HttpPost("CreateVariant")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateVariant(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ;// return await Create();
            }

            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            ProductVariant productVariant = new ProductVariant()
            {
                Id = new Guid(),
                ColorId = viewModel.VariantColorId,
                Remarks = viewModel.VariantName,
                SizeId = viewModel.VariantSizeId,
                UserId= userId,
                Price = viewModel.Price,
                Isactive = true
            };
                _context.ProductVariants.Add(productVariant);
                _context.SaveChanges();
           

            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //wwait _fileService.UpdateAsync(newId);
            return RedirectToAction("create","Products");
        }

        [Authorize]
        [HttpGet("DeleteVariant")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteVariant(Guid id)
        {
            if (!ModelState.IsValid)
            {
                ;// return await Create();
            }

            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var variant = _context.ProductVariants.FirstOrDefault(x => x.Id == id);
            _context.Remove(variant);
            _context.SaveChanges();


            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);
            //Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //wwait _fileService.UpdateAsync(newId);
            return RedirectToAction("create", "Products");
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