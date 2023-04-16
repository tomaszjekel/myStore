using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Infrastructure.EF;
using MyStore.Services.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MyStore.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly MyStoreContext _context;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, IFileService fileService,
                IMapper mapper, MyStoreContext context)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _fileService = fileService;
            _mapper = mapper;
            _context = context;
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
        public async Task<PaginatedList<Product>> BrowseByUserId(string name, int? pageIndex, Guid userId, Guid? category)
        {
            var products = await _productRepository.BrowseByUserId(name, pageIndex, userId, category);
            int pageSize = 9;

            var Products = await PaginatedList<Product>.CreateAsync(
                  products, pageIndex ?? 1, pageSize);

            return Products;

        }

        public async Task<PaginatedList<Product>> BrowseByPrice(string name, int? pageIndex, int? startPrice, int? endPrice)
        {
            IQueryable<Product> p;
            
            
            if (endPrice == 0)
                p = _context.Products.Where(x => x.Price >= startPrice && x.Deleted == false).
                    Include(x=>x.Variants).Include(x=>x.Files);
            else
                p = _context.Products.Where(x => x.Price >= startPrice && x.Price <= endPrice && x.Deleted == false)
                    .Include(x => x.Variants).Include(x => x.Files);
            
            
               
            int pageSize = 9;

            var Products = await PaginatedList<Product>.CreateAsync(
                  p, pageIndex ?? 1, pageSize);

            return Products;

        }

        public async Task<PaginatedList<Product>> BrowseAsync1(string name, int? pageIndex, Guid userId)
        {
            var products = await _productRepository.BrowseAsync(name);
            int pageSize = 3;

            var Products = await PaginatedList<Product>.CreateAsync(
                  products, pageIndex ?? 1, pageSize);

            return Products;
        }

        public async Task CreateAsync(Product p)
        {
            await _productRepository.CreateAsync(p);
        }

        public async Task DeleteImageFromProduct(Guid productId, Guid imageId, Guid userId)
        {
            await _productRepository.DeleteImageFromProduct(productId, imageId,  userId);
           
        }

        public async Task DeleteImage(string imageName, Guid userId)
        {
            await _productRepository.DeleteImage(imageName, userId);

        }

        public async Task<List<string>> UploadandResize(ICollection<IFormFile> files, Guid userId, Guid productId)
        {

            List<string> pathImage =  new List<string>();

            var filesPath = Environment.GetEnvironmentVariable("UPLOAD_DIR");

            foreach (var file in files)
            {
                Guid fileNameGuid = Guid.NewGuid();
                if (file.Length > 0)
                {
                    //using (var fileStream = new FileStream(Path.Combine($"{filesPath}", file.FileName), FileMode.Create))
                    using (var fileStream = new FileStream(Path.Combine($"{filesPath}", fileNameGuid.ToString() + Path.GetExtension(file.FileName)), FileMode.OpenOrCreate))
                    {
                        pathImage.Add(fileNameGuid.ToString() + Path.GetExtension(file.FileName));

                        file.CopyTo(fileStream);
                        string fileName = fileNameGuid.ToString() + Path.GetExtension(file.FileName);
                        if(productId == new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            await _fileService.CreateAsync(userId, null, fileName, DateTime.Now);
                        }
                        else
                        {
                            await _fileService.CreateAsync(userId, productId, fileName, DateTime.Now);
                        }
                    }
                }
            }
            foreach (var imageName in pathImage)
            {
                using (var stream = new FileStream(filesPath + "/" + imageName, FileMode.Open))
                {
                    using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(stream))
                    {
                        if (image.Width > image.Height)
                        {
                            image.Mutate(x => x
                             .Resize(720, 0));
                        }
                        else
                        {
                            image.Mutate(x => x
                            .Resize(480, 0));
                        }

                        using (var minFileStream = new FileStream(filesPath + "/" + "min_" + imageName, FileMode.Create))
                        {
                            image.SaveAsPng(minFileStream);
                        }
                    }
                }
            }
            return pathImage;
        }

        public async Task UpdateProduct(Product p)
        {
          await  _productRepository.UpdateProduct(p);
        }

        public Task<PaginatedList<Product>> BrowseAsync1(string name, int? pageIndex)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteProduct(Guid productId, Guid userId)
        {
            await _productRepository.DeleteProduct(productId,userId);
        }
        public Task<List<Cities>> GetCities()
        {
            return _productRepository.GetCities();
        }

    }
}