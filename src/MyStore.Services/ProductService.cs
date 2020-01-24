using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Services.DTO;

namespace MyStore.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
        public ProductService(IProductRepository productRepository, IUserRepository userRepository,
                IMapper mapper)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        
        public async Task<ProductDto> GetAsync(Guid id)
        {
            var product = await _productRepository.GetAsync(id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> BrowseAsync(string name)
        {
            var products = await _productRepository.BrowseAsync(name);
            int pageSize = 3;
            int? pageIndex = 1;
            var Products = await PaginatedList<Product>.CreateAsync(
                  products, pageIndex ?? 1, pageSize);

            return products.Select(x => new ProductDto
            {
                Category = x.Category,
                Description = x.Description,
                Files = x.Files,
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                UserId = x.UserId
            }).ToList();
        }

        public async Task<PaginatedList<Product>> BrowseAsync1(string name, int? pageIndex)
        {
            var products = await _productRepository.BrowseAsync(name);
            int pageSize = 3;

            var Products = await PaginatedList<Product>.CreateAsync(
                  products, pageIndex ?? 1, pageSize);

            return Products;
        }

        public async Task CreateAsync(Guid id, Guid userId, string name, string category, decimal price, string description)
        {
            var product = new Product(id, userId, name, category, price, description);
            await _productRepository.CreateAsync(product);
        }
    }
}