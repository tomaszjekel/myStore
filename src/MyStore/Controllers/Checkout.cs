using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Stripe.Checkout;

namespace MyStore.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            //var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            //ViewBag.cart = cart;
            //ViewBag.DollarAmount = cart.Sum(item => item.Pet.Price * item.Quantity);
            //ViewBag.total = Math.Round(ViewBag.DollarAmount, 2) * 100;
            //ViewBag.total = Convert.ToInt64(ViewBag.total);
            //long total = ViewBag.total;
            //TotalAmount = total.ToString();

            //TempData["TotalAmount"] = TotalAmount;
            
            return View();
        }
    }
}
