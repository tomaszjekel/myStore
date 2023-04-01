using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyStore.Domain;
using MyStore.Services.DTO;

namespace MyStore.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetAsync(Guid id);
        Task<IEnumerable<ProductDto>> BrowseAsync(string name = "");
        Task<PaginatedList<Product>> BrowseAsync1(string name, int? pageIndex);
        Task<PaginatedList<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId, Guid? category);
        Task CreateAsync(Product p);
        Task DeleteImageFromProduct(Guid productId,Guid imageId, Guid userId);
        Task DeleteImage(string imageName, Guid userId);

        Task<List<string>> UploadandResize(ICollection<IFormFile> files, Guid userId, Guid productId);
        Task UpdateProduct(Product p);
        Task DeleteProduct(Guid productId, Guid userId);
        Task<List<Cities>> GetCities();
           
    }
}