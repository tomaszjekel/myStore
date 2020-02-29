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
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier),  out userGuid);
            var products = await _productService.BrowseByUserId(keyword, pageIndex,userId);
           

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
                    Files= p.Files
                });
            //if (userGuid != Guid.Empty && name !="all")
            //    viewModels = viewModels.Where(c => c.ProductUserId == userGuid);

            ProductNewViewModel newModel = new ProductNewViewModel();
            newModel.Products =  viewModels.ToList();
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

        [HttpGet("{id}/details")]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _productService.GetAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var fileList = await _fileService.BrowseByProductAsync(product.UserId, product.Id);

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Files = product.Files,
                Description = product.Description
            };

            return View(viewModel);
        }

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

            var viewModel = new CreateProductViewModel() { Files=fileList.ToList() };
            
            return View(viewModel);
        }

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
            viewModel.Category, viewModel.Price, viewModel.Description);

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

        
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _productService.GetAsync(id);
            if (product != null)
            {
                EditProductViewModel edit = new EditProductViewModel
                {
                    Id= product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Files = product.Files.Select(x => new FileDto {Id= x.Id, Name = x.Name, ProductId = x.ProductId }).ToList(),
                    Category= product.Category
                };

                return View(edit);
            }

            return NotFound();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Edit(EditProductViewModel editModel)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid)
            {
                return  await Edit(editModel.Id);

            }
            if (editModel != null)
            {
                await _productService.UpdateProduct(editModel.Id,editModel.Name, editModel.Price, editModel.Category,editModel.Description);

                //return View(edit);
                //return RedirectToAction(editModel.Id.ToString(), "Products");
                return RedirectToAction("browse", new { userId = userId });
            }

            return NotFound();
        }

        [HttpGet("/Delete/{imageId}/{productId}")]
        public async Task<IActionResult> Delete(Guid imageId, Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteImageFromProduct(productId ,imageId, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
           return RedirectToAction(productId.ToString(), "Products");
        
        }

        [HttpGet("/DeleteImage/{imageName}")]
        public async Task<IActionResult> DeleteImage(string imageName, Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteImage(imageName, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
            return RedirectToAction("create");

        }

        [HttpGet("/DeleteProduct/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            Guid userId = new Guid(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _productService.DeleteProduct(productId, userId);

            //return RedirectToAction( productId.ToString(),"Products" );
            return RedirectToAction("browse",new { userId = userId});

        }


        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> Post([FromBody] CreateProduct request)
        {
            await _productService.CreateAsync(Guid.NewGuid(), request.UserId, request.Name,
                request.Category, request.Price, request.Description);

            return Ok();
        }

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
            else
            {
                await _productService.UploadandResize(files, userGuid, productId );
               return RedirectToAction(productId.ToString(), "Products");
            }
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