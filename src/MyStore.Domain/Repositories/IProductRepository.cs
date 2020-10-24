﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStore.Domain.Repositories
{
    public interface IProductRepository
    {
        //CQS - Command & Query separation
        Task<Product> GetAsync(Guid id);
        Task<IQueryable<Product>> BrowseAsync(string name);
        Task CreateAsync(Product product);
        Task DeleteImageFromProduct(Guid productId, Guid imageId, Guid userId);
        Task DeleteImage( string imageName, Guid userId);
        Task UpdateProduct(Product p);
        Task<IQueryable<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId);
        Task DeleteProduct(Guid productId, Guid userId);
        Task<List<Cities>> GetCities();
        Task<List<Category>> GetCategories();
        void RemoveCategory(int id);
        void CreateCategory(string name);
        void AddToBasket(Guid productId, int quantity, Guid userId);
        Task<List<BasketItem>> GetBasket(Guid userGuid);
        Task<Item> GetBasketItem(int basketItemId);
        void ChangeQuantity(int basketItemId, int quantity);
        void RemoveBasketItem(int id);
    }
}