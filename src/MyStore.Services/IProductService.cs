﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyStore.Domain;
using MyStore.Services.DTO;

namespace MyStore.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetAsync(Guid id);
         Task<IEnumerable<ProductDto>> BrowseAsync(string name = "");
        Task<PaginatedList<Product>> BrowseAsync1(string name, int? pageIndex);
        Task CreateAsync(Guid id, Guid userId, string name,
            string category, decimal price, string description);
    }
}