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

        public async Task<IEnumerable<ProductDto>> BrowseAsync(string name = "")
        {
            var products = await _productRepository.BrowseAsync(name);

            return products.Select(_mapper.Map<ProductDto>);
        }

        public async Task CreateAsync(Guid id, Guid userId, string name, string category, decimal price, string description)
        {
            var product = new Product(id, userId, name, category, price, description);
            await _productRepository.CreateAsync(product);
        }
    }
}