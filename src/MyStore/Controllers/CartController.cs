using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyStore.Domain;
using MyStore.Models;
using MyStore.Services;

namespace MyStore.Controllers
{
    public class CartController : BaseController
    {
        private ICartService _cart;
        public CartController(ICartService cart)
        {
            _cart = cart;
        }
        List<CartItem> listItem = new List<CartItem>();

        public IActionResult Index()
        {
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var currentCart =  _cart.GetCart(userGuid);

            //_card.AddProductAsync(userGuid, new Guid("96dddda1-28cf-42ed-b472-03daaa0c6deb
            var listItem = new List<CartItem>();
            listItem.Add(new CartItem { ProductId = new Guid("96dddda1-28cf-42ed-b472-03daaa0c6deb") });

            //Cart cart = new Cart {  UserId = userGuid } ;

            return View();
        }

        [HttpGet("Add/{productId}")]
        public IActionResult Add(string productId)
        {
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var currentCart = _cart.GetCart(userGuid);

            
            var currentCartIteam = _cart.SetCartItem(Guid.Parse(productId),currentCart.Id);
            var currentCartIteams = _cart.GetCartItems(currentCart.Id);
            listItem.AddRange(currentCartIteams);
            currentCart.Items = listItem;

            //return Json(new {cart = currentCart });
            return RedirectToAction("browse");
        }

        [HttpGet("Browse")]
        public IActionResult Browse()
        {
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var currentCart = _cart.GetCart(userGuid);
            var currentCartIteams = _cart.GetCartItems(currentCart.Id);
            CartViewModel cartViewModel = new CartViewModel();
            cartViewModel.CartItems = currentCartIteams;
            
            return View(cartViewModel);
        }

        [HttpGet("Delete/{cartItemId}")]
        public IActionResult Delete(string cartItemId)
        {
            _cart.RemoveCartItem(cartItemId);
            Guid userGuid;
            Guid.TryParse(this.User.FindFirstValue(ClaimTypes.NameIdentifier), out userGuid);
            var currentCart = _cart.GetCart(userGuid);
            var currentCartIteams = _cart.GetCartItems(currentCart.Id);
            CartViewModel cartViewModel = new CartViewModel();
            cartViewModel.CartItems = currentCartIteams;

            return RedirectToAction("browse");
        }

    }
}