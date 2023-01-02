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
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            if (cart != null)
            {
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Price * item.Quantity);

            }
            return View();
        }
        private int IsExist(Guid Id)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Id.Equals(Id))
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
          

            var pet = _context.Products.FirstOrDefault(m => m.Id == Guid.Parse(id));
            List<Product> cart;
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                cart = new List<Product>();
                cart.Add(await _context.Products.FindAsync(Guid.Parse(id)));
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
                int index = IsExist(Guid.Parse(id));
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add( await _context.Products.FindAsync(Guid.Parse(id)));
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            //List<Product> cart1 = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            return Json(new { qty = cart.Sum(item => item.Quantity), price= cart.Sum(item => item.Price * item.Quantity) });
        }
        public IActionResult Remove(Guid Id)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            int index = IsExist(Id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");



        }

        [HttpGet]
        public JsonResult Quantity()
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            int qty = 0;
            if (cart != null)
            {
                return Json(new {qty= cart.Sum(item => item.Quantity), price = cart.Sum(item => item.Price * item.Quantity) });
            }
            return Json(new { qty = 0, price = 0 });
        }
    }
}
