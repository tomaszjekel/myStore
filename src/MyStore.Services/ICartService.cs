using MyStore.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public interface ICartService
    {
        Task AddProduct(Guid userId, Guid productId);
        Task AddProductAsync(Guid userId, Guid productId);
        Task CreateAsync(Guid userId);
        Task DeleteAsync(Guid userId);
        Cart GetCart(Guid userGuid);
        CartItem SetCartItem(Guid guid, int id);
        List<CartItem> GetCartItems(int id);
        void RemoveCartItem(string cartItemId);
    }
}