using System;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Domain.Repositories;

namespace MyStore.Services
{
    public class CartService : ICartService
    {
        private readonly ICartProvider _cartProvider;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public CartService(ICartProvider cartProvider,
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _cartProvider = cartProvider;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        
        public async Task AddProductAsync(Guid userId, Guid productId)
        {
            var product = await _productRepository.GetAsync(productId);
            if (product == null)
            {
                throw new Exception($"Product with id: {productId} was not found.");
            }
            var cart = _cartProvider.Get(userId);
            cart.AddProduct(product);
            _cartProvider.Update(userId, cart);
        }

        public async Task CreateAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            _cartProvider.Update(userId, new Cart(user));
        }

        public async Task DeleteAsync(Guid userId)
        {
            await Task.CompletedTask;
            _cartProvider.Delete(userId);
        }
    }
}