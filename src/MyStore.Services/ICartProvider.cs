using System;
using MyStore.Domain;

namespace MyStore.Services
{
    public interface ICartProvider
    {
        Cart Get(Guid userId);
        void Update(Guid userId, Cart cart);
        void Delete(Guid userId);
    }
}