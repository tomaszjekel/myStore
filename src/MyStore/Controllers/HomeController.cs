using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyStore.Domain;
using MyStore.Framework;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppOptions _appOptions;
        private readonly IMemoryCache _cache;
        private IProductService _productService;

        public HomeController(IProductService productService,AppOptions appOptions, IMemoryCache cache)
        {
            _productService = productService;
            _appOptions = appOptions;
            _cache = cache;
        }

        public IActionResult Index()
        {
            //return View();
            return RedirectToAction("browse", "Products");
        }

        public IActionResult About()
        {
            var message = _cache.Get<string>("message");
            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"Welcome to {_appOptions.StoreName}.";
                _cache.Set("message", message);
            }
            ViewData["Message"] = message;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [Authorize]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("basket")]
        public async Task<IActionResult> Basket()
        {


                Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var cookies = HttpContext.Request.Cookies["ConstoCookie"];
            List<BasketItemViewModel> basketViewModelList = new List<BasketItemViewModel>();
            List<BasketItem> basket = await _productService.GetBasket(userGuid);
                    foreach(var b in basket)
            {

                basketViewModelList.Add(
                    new BasketItemViewModel
                    {
                        ItemViewModel = new ItemViewModel
                        {
                            Id = b.Item.Id,
                            Price = b.Item.Price,
                            ProductViewModel = new ProductViewModel
                            {
                                Id = b.Item.Product.Id,
                                Price = b.Item.Product.Price,
                                Category = b.Item.Product.Category,
                                Description = b.Item.Product.Description,
                                Name = b.Item.Product.Name,
                                Quantity = b.Item.Quantity,
                                Files = b.Item.Product.Files
                                

                            }
                        },
                        UserId=b.UserId,
                        Id = b.Id
                    });
            }
            


            return View(basketViewModelList);
        }

        [HttpGet("deleteFromBasket/{id}")]
        public async Task<IActionResult> DeleteFromBasket(int id)
        {
            _productService.RemoveBasketItem(id);

                return RedirectToAction("basket", "Home");
        }

        [HttpGet("date")]
        [ResponseCache(Duration = 10, 
            VaryByHeader = "x-custom", 
            VaryByQueryKeys = new []{"q1"})]
        public IActionResult GetDate()
            => Content(DateTime.Now.ToString(CultureInfo.InvariantCulture));

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
