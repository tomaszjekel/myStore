using System;
using Microsoft.Extensions.Caching.Memory;
using MyStore.Domain;

namespace MyStore.Services
{
    public class CartProvider : ICartProvider
    {
        private readonly IMemoryCache _cache;

        public CartProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Cart Get(Guid userId)
            => _cache.Get<Cart>(GetKey(userId));

        public void Update(Guid userId, Cart cart)
            => _cache.Set(GetKey(userId), cart, TimeSpan.FromMinutes(10));

        public void Delete(Guid userId)
            => _cache.Remove(GetKey(userId));

        private string GetKey(Guid userId) => $"user:{userId}:cart";
    }
}