using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Infrastructure.EF;

namespace MyStore.Services
{
    public class CartService : ICartService
    {
        private readonly ICartProvider _cartProvider;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly MyStoreContext _context;

        public CartService(ICartProvider cartProvider,
            IProductRepository productRepository,
            IUserRepository userRepository,
            MyStoreContext context
            )
        {
            _cartProvider = cartProvider;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _context = context;

        }
        
        public async Task AddProduct(Guid userId, Guid productId)
        {
            
        }

        public Cart GetCart(Guid userGuid) {
            var cart =  _context.Cart.Where(x => x.UserId == userGuid).FirstOrDefault();
            if (cart == null)
            {
                _context.Cart.Add(new Cart { UserId = userGuid });
                _context.SaveChanges();
            }
            cart =  _context.Cart.Where(x => x.UserId == userGuid).FirstOrDefault();
            return cart;
        }
        public CartItem SetCartItem(Guid productId, int cartId)
        {
            var cartItem = new CartItem { ProductId = productId, CartId = cartId };
            _context.CartItem.Add(cartItem);
            _context.SaveChanges();
            return _context.CartItem.Where(x => x.ProductId == productId && x.CartId ==cartId).FirstOrDefault();
        }

        public List<CartItem> GetCartItems(int cartId)
        {
            return _context.CartItem.Where(x => x.CartId == cartId).Include(x=>x.Product).Include(x=>x.Product.Files).ToList();
        }
       public void RemoveCartItem(string cartItemId)
        {
            var cartItem = _context.CartItem.Where(x => x.Id == Int32.Parse(cartItemId)).FirstOrDefault();
            _context.CartItem.Remove(cartItem);
            _context.SaveChanges();
        }

        public async Task AddProductAsync(Guid userId, Guid productId)
        {
            var product = await _productRepository.GetAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with id: {productId} was not found.");
            }
            
            var cart = _cartProvider.Get(userId);
           // cart.AddProduct(product);
            _cartProvider.Update(userId, cart);
        }

        public async Task CreateAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
 
        }

        public async Task DeleteAsync(Guid userId)
        {
            await Task.CompletedTask;
            _cartProvider.Delete(userId);
        }
    }
}