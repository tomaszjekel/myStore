﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Stripe.Checkout;
using MyStore.Domain.Repositories;
using MyStore.Infrastructure.EF;
using MyStore.Domain;
using MyStore.Helper;
using System.Linq;
using MyStore.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace MyStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MyStoreContext _context;
        public CheckoutController(MyStoreContext context)
        {
            _context = context;
        }

        public IActionResult Create(OrderViewModel model)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            model.Cart = cart;

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            Addresses adres = new Addresses
            {
                Address = model.Address,
                Address1 = model.Address1,
                City = model.City,
                Country = model.Country,
                Email = model.Email,
                FistName = model.FirstName,
                Id = new Guid(),
                LastName = model.LastName,
                OrderNotes = model.OrderNotes,
                Phone = model.Phone,
                Postcode = model.Postcode,
                State = model.State

            };


            List<OrderItem> orderItemList = new List<OrderItem>();
            for (int i = 0; i < cart.Count; i++)
            {
                var cartItem = new OrderItem
                {
                    ProductId = cart[i].ProductId,
                    ProductName = cart[i].ProductName,
                    UnitPrice = cart[i].UnitPrice,
                    Quantity = cart[i].Quantity,
                    Id = new Guid(),
                    Size = cart[i].Size,
                    SizeId = cart[i].SizeId,
                };

                orderItemList.Add(cartItem);
            }


            var order = new Order
            {
                Id = new Guid(),
                CreatedAt = DateTime.Now,
                Items = orderItemList,
                TotalPrice = orderItemList.Sum(x => x.UnitPrice),
                UserId = new Guid(),
                Address = adres,
                Completed = false
            };

            _context.Orders.AddAsync(order);
            _context.SaveChanges();

            //List<CartItem> cart1 = new List<CartItem>();

            //SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart1);

            model.AllOrder = order;
            ViewBag.Order = "OK";
            return View(model);
        }
        public IActionResult Index()
        {
            OrderViewModel model = new OrderViewModel();
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            model.Cart = cart;
            //var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            //ViewBag.cart = cart;
            //ViewBag.DollarAmount = cart.Sum(item => item.Pet.Price * item.Quantity);
            //ViewBag.total = Math.Round(ViewBag.DollarAmount, 2) * 100;
            //ViewBag.total = Convert.ToInt64(ViewBag.total);
            //long total = ViewBag.total;
            //TotalAmount = total.ToString();

            //TempData["TotalAmount"] = TotalAmount;

            return View(model);
        }

        //[HttpPost("create-checkout-session")]
        [HttpPost]
        public ActionResult CreateCheckoutSession1(string orderid)
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
                SuccessUrl = "https://tomo24.pl/Checkout/complete?orderid=" + orderid,
                CancelUrl = "http://localhost:5000/Home/cancel",
            };

            Guid g = Guid.Parse(orderid);
            var order = _context.Orders.Where(p => p.Id == g).Include(x => x.Items).FirstOrDefault();

            foreach (var item in order.Items)
            {

                var currentLineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = item.UnitPrice * 100,//item.Quantity,
                        Currency = "pln",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,

                            //Images = "images/"+item.
                        },
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(currentLineItem);
            }
            var currentLineItemShipping = new Stripe.Checkout.SessionLineItemOptions
            {
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = 15 * 100,//item.Quantity,
                    Currency = "pln",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Wysyłka",

                        //Images = "images/"+item.
                    },
                },
                Quantity = 1,
            };
            options.LineItems.Add(currentLineItemShipping);
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult Complete(string orderid)
        {
            var order = _context.Orders.Where(p => p.Id == new Guid(orderid)).Include(x => x.Items).FirstOrDefault();

            order.Completed = true;
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();

            foreach (var item in order.Items)
            {
                var product = _context.Products.Where(x => x.Id == item.ProductId).Include(x => x.Variants).FirstOrDefault();
                if (item.SizeId != null)
                {
                    product.Variants.Where(x => x.SizeId == item.SizeId).FirstOrDefault().Quantity -= item.Quantity;
                    if (product.Variants.Where(x => x.SizeId == item.SizeId).Select(x => x.Quantity).FirstOrDefault() < 0)
                        product.Variants.Where(x => x.SizeId == item.SizeId).FirstOrDefault().Quantity = 0;
                }
                else
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity < 0)
                        product.Quantity = 0;
                }

                _context.Update(product);
            }

            _context.SaveChanges();

            List<CartItem> cart1 = new List<CartItem>();
            SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart1);
            return RedirectToAction("Completed", "Checkout"); ;
        }
        public IActionResult Completed()
        {
            return View();
        }
    }
}
