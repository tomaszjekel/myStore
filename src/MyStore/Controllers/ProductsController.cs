using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("browse")]
        public async Task<IActionResult> Browse(string name)
        {
            var products = await _productService.BrowseAsync(name);
            var viewModels = products.Select(p =>
                new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price
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

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };

            return View(viewModel);
        }

        [HttpGet("create")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Create()
        {

            
            var viewModel = new CreateProductViewModel();
            
            return View(viewModel);
        }

        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create(CreateProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
                if (claimsIdentity != null && claimsIdentity.FindFirst("UserId").Value != null)
                {
                    Guid userId = new Guid(claimsIdentity.FindFirst("UserId").Value);
                    await _productService.CreateAsync(Guid.NewGuid(), userId, viewModel.Name,
                        viewModel.Category, viewModel.Price);
                }
            }
            return RedirectToAction(nameof(Browse));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BrowseProducts query)
        {
            var products = await _productService.BrowseAsync(query.Name);

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productService.GetAsync(id);
            if (product != null)
            {
                return Ok(product);
            }

            return NotFound();
        }

        [HttpPost]
        [ModelValidationFilter]
        public async Task<IActionResult> Post([FromBody] CreateProduct request)
        {
            await _productService.CreateAsync(Guid.NewGuid(), request.UserId, request.Name,
                request.Category, request.Price);

            return Ok();
        }
    }

    public class CreateProduct
    {
        [Required]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }

    public class BrowseProducts
    {
        public string Name { get; set; }
    }
}