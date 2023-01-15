using Microsoft.AspNetCore.Mvc;
using MyStore.Domain;
using MyStore.Helper;
using MyStore.Infrastructure.EF;
using MyStore.Models;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreContext _context;
        public CartController(MyStoreContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.IsShopingCart = "Shoping Cart";
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            if (cart != null)
            {
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.TotalPrice * item.Quantity);

            }
            return View();
        }
        private int IsExist(Guid Id)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].ProductId.Equals(Id))
                {
                    return i;
                }
            }
            return -1;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Products.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            return View(pet);
        }
        
        public async Task<JsonResult> BuyAsync(string id)
        {
          

            List<CartItem> cart;
            var product = _context.Products.FirstOrDefault(m => m.Id == Guid.Parse(id));
            var prod = await _context.Products.FindAsync(Guid.Parse(id));
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                cart = new List<CartItem>();
                cart.Add(new CartItem { ProductId = prod.Id, Img = prod.Img, ProductName = prod.Name, Quantity = 1, UnitPrice = prod.Price });
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                int index = IsExist(Guid.Parse(id));
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new CartItem { ProductId = prod.Id, Img = prod.Img, ProductName = prod.Name, Quantity = 1, UnitPrice = prod.Price });
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            //List<Product> cart1 = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            return Json(new { qty = cart.Sum(item => item.Quantity), price= cart.Sum(item => item.UnitPrice * item.Quantity) });
        }
        public IActionResult Remove(Guid Id)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            int index = IsExist(Id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");



        }

        [HttpGet]
        public JsonResult Quantity()
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            int qty = 0;
            if (cart != null)
            {
                return Json(new {qty= cart.Sum(item => item.Quantity), price = cart.Sum(item => item.UnitPrice * item.Quantity) });
            }
            return Json(new { qty = 0, price = 0 });
        }

        public async Task<JsonResult> BuyQty(string id, int qty)
        {


            List<CartItem> cart;
            var product = _context.Products.FirstOrDefault(m => m.Id == Guid.Parse(id));
            var prod = await _context.Products.FindAsync(Guid.Parse(id));
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                cart = new List<CartItem>();
                cart.Add(new CartItem { ProductId = prod.Id, Img = prod.Img, ProductName = prod.Name, Quantity = qty, UnitPrice = prod.Price });
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                int index = IsExist(Guid.Parse(id));
                if (index != -1)
                {
                    cart[index].Quantity+=qty;
                }
                else
                {
                    cart.Add(new CartItem { ProductId = prod.Id, Img = prod.Img, ProductName = prod.Name, Quantity = qty, UnitPrice = prod.Price });
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            //List<Product> cart1 = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            return Json(new { qty = cart.Sum(item => item.Quantity), price = cart.Sum(item => item.UnitPrice * item.Quantity) });
        }
    }
}
