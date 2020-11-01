using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyStore.Domain;
using MyStore.Models;
using MyStore.Services;
using System.Collections;

namespace MyStore.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {

        private readonly IProductService _productService;
        private readonly IFileService _fileService;

        public CartController(IProductService productService, IFileService fileService)
        {
            _productService = productService;
            _fileService = fileService;
        }


        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            //ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
           
            List<ItemViewModel> itemViewModelsList = new List<ItemViewModel>();
            if (cart != null)
            {
                foreach (var c in cart)
                {
                    itemViewModelsList.Add(new ItemViewModel
                    {
                        Id = c.Id,
                        Price = c.Price,
                        Quantity = c.Quantity,
                        ProductViewModel = new ProductViewModel
                        {
                            Price = c.Product.Price,
                            Id = c.Product.Id,
                            Category = c.Product.Category,
                            Description = c.Product.Description,
                            Name = c.Product.Name,
                            Files = await _fileService.BrowseByProductByIdAsync(c.Product.Id)
                        }
                    });
                }
            }
            return View(itemViewModelsList);
        }

        [Route("buy/{id}/{quantity}")]
        public async Task<IActionResult> Buy(Guid id, int quantity)
        {
            var productDto = await _productService.GetAsync(id);
            Product product = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Category = productDto.Category,
                CityId = productDto.CityId,
                Description = productDto.Description,
                Files = productDto.Files,
                Price = productDto.Price,
                UserId = productDto.UserId
            };
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {

                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = product , Quantity = quantity });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity+=quantity;
                }
                else
                {
                    cart.Add(new Item { Product = product, Quantity = quantity });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(Guid id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(Guid id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                for (int i = 0; i < cart.Count; i++)
                {
                    if (cart[i].Product.Id.Equals(id))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        [Route("Quantity/{id}")]
        public IActionResult Quantity(Guid id, int quantity)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart[index].Quantity = quantity;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        [Route("GetQuantity")]
        public IActionResult GetQuantity()
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            return Json(cart?.Count()??0);
        }

    }
}
