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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        private int IsExist(Guid Id, Guid? sizeId)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if(sizeId== null)
                    if (cart[i].ProductId.Equals(Id))
                    {
                        return i;
                    }
                    
                if (cart[i].ProductId.Equals(Id) && cart[i].SizeId.Equals(sizeId))
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
            var prod = _context.Products.Where(p => p.Id == Guid.Parse(id)).Include(x => x.Files).FirstOrDefault();
            //var prod = await _context.Products.FindAsync(Guid.Parse(id));
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                cart = new List<CartItem>();
                if (prod.Quantity == 0)
                {
                    return Json(
                        new
                        {
                            qty = cart.Sum(item => item.Quantity),
                            price = cart.Sum(item => item.UnitPrice * item.Quantity),
                            maxSizeQuantity = prod.Quantity
                        });
                }
                else
                    cart.Add(new CartItem { ProductId = prod.Id, Img = prod.Files.FirstOrDefault().Name, ProductName = prod.Name, Quantity = 1, UnitPrice = prod.Price });
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
            }
            else
            {   
                cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                int index = IsExist(Guid.Parse(id), null);
                if (index != -1)
                {
                    if(cart[index].Quantity >= prod.Quantity)
                    {
                        return Json(
                            new { 
                                qty = cart.Sum(item => item.Quantity),
                                price = cart.Sum(item => item.UnitPrice * item.Quantity),
                                maxSizeQuantity = prod.Quantity
                            });
                    }else
                        cart[index].Quantity++;
                }
                else
                {
                    if(prod.Quantity != 0)
                        cart.Add(
                            new CartItem 
                            {
                                ProductId = prod.Id,
                                Img = prod.Files.Where(x=>x.IsDefault == true).FirstOrDefault().Name,
                                ProductName = prod.Name,
                                Quantity = 1, 
                                UnitPrice = prod.Price 
                            });
                    else
                    {
                        return Json(
                            new
                            {
                                qty = cart.Sum(item => item.Quantity),
                                price = cart.Sum(item => item.UnitPrice * item.Quantity),
                                maxSizeQuantity = prod.Quantity
                            });
                    }
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            //List<Product> cart1 = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            return Json(new { qty = cart.Sum(item => item.Quantity), price= cart.Sum(item => item.UnitPrice * item.Quantity) });
        }
        public IActionResult Remove(Guid Id)
        {
            List<CartItem> cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            int index = IsExist(Id, null);
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

        public async Task<JsonResult> BuyQty(string id, int qty, Guid? sizeId)
        {
            List<CartItem> cart;
            var prod = _context.Products.Where(p => p.Id == Guid.Parse(id)).Include(x => x.Files).Include(x=>x.Variants).FirstOrDefault();
            var productQuantity = prod.Variants.Where(x => x.SizeId == sizeId).Select(x=>x.Quantity).FirstOrDefault();
            if(productQuantity == null)
            {
                productQuantity = prod.Quantity;
            }
            if (SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart") == null)
            {
                cart = new List<CartItem>();
                
                if (qty > productQuantity)
                {
                    return Json(
                        new
                        {
                            qty = cart.Sum(item => item.Quantity),
                            price = cart.Sum(item => item.UnitPrice * item.Quantity),
                            maxSizeQuantity = productQuantity
                        });
                }
                else
                {
                    cart.Add(new CartItem 
                    { 
                        ProductId = prod.Id,
                        Img = prod.Files.Where(x => x.IsDefault == true).FirstOrDefault().Name,
                        ProductName = prod.Name,
                        Quantity = qty,
                        UnitPrice = prod.Price, 
                        Size =_context.Sizes.Where(x=>x.Id == sizeId).Select(x=>x.Name).FirstOrDefault(),
                        SizeId = sizeId
                    });
                    SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);
                }
            }
            else
            {
                cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
                int index = IsExist(Guid.Parse(id), sizeId);
                if (index != -1)
                {

                    if (cart[index].Quantity+qty > productQuantity)
                    {
                        return Json(
                            new { 
                                qty = cart.Sum(item => item.Quantity), 
                                price = cart.Sum(item => item.UnitPrice * item.Quantity), 
                                maxSizeQuantity = productQuantity
                            });
                    }else
                        cart[index].Quantity+=qty;
                }
                else
                {
                    if (qty > productQuantity)
                    {
                        return Json(
                            new
                            {
                                qty = cart.Sum(item => item.Quantity),
                                price = cart.Sum(item => item.UnitPrice * item.Quantity),
                                maxSizeQuantity = productQuantity
                            });
                    }else
                        cart.Add(
                            new CartItem 
                            { 
                                ProductId = prod.Id,
                                Img = prod.Files.Where(x => x.IsDefault == true).FirstOrDefault().Name,
                                ProductName = prod.Name,
                                Quantity = qty,
                                UnitPrice = prod.Price,
                                Size = _context.Sizes.Where(x => x.Id == sizeId).Select(x => x.Name).FirstOrDefault(),
                                SizeId = sizeId
                            });
                }
                SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            }
            //List<Product> cart1 = SessionHelper.GetObjectFromJson<List<Product>>(HttpContext.Session, "cart");
            return Json(
                new { 
                    qty = cart.Sum(item => item.Quantity), 
                    price = cart.Sum(item => item.UnitPrice * item.Quantity),
                    maxSizeQuantity = -1
                });
        }

        public async Task<JsonResult> ChangeQtyCard(string id, int qty, string sizeId)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
            var product = _context.Products.Where(x => x.Id == Guid.Parse(id)).Include(x=>x.Variants).FirstOrDefault();
            var productQuantity = product.Variants.Where(x => x.SizeId == Guid.Parse(sizeId)).Select(x => x.Quantity).FirstOrDefault();
            if(productQuantity == null) {
                productQuantity = product.Quantity;
            }
            if(qty > productQuantity)
            {
                return Json(
                new
                {
                    qty = cart.Sum(item => item.Quantity),
                    price = cart.Sum(item => item.UnitPrice * item.Quantity),
                    maxSizeQuantity = productQuantity
                });
            }else
                if(sizeId!=null)
                    cart.Where(x=>x.ProductId.ToString()==id && x.SizeId == new Guid( sizeId)).FirstOrDefault().Quantity = qty;
                else
                    cart.Where(x => x.ProductId.ToString() == id).FirstOrDefault().Quantity = qty;

            SessionHelper.SetObjectasJson(HttpContext.Session, "cart", cart);

            return Json(
                new {
                    qty = cart.Sum(item => item.Quantity), 
                    price = cart.Sum(item => item.UnitPrice * item.Quantity),
                    maxSizeQuantity = -1
                });
        }
    }
}
