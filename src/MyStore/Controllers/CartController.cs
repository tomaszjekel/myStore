using Microsoft.AspNetCore.Mvc;
using MyStore.Domain;
using MyStore.Helper;
using MyStore.Infrastructure.EF;
using MyStore.Models;
using System;
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

        public async Task<IActionResult> Buy(Guid Id)
        {

            //    ProductModel productModel = new ProductModel();

            var pet = _context.Products.FirstOrDefault(m => m.Id == Id);

            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                List<Product> cart = new List<Product>();
                cart.Add(await _context.Products.FindAsync(Id));
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
                int index = IsExist(Id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add( await _context.Products.FindAsync(Id));
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(Guid Id)
        {
            List<Product> cart = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            int index = IsExist(Id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");



        }
    }
}
