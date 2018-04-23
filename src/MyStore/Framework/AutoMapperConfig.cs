using AutoMapper;
using MyStore.Domain;
using MyStore.Services.DTO;
  
namespace MyStore.Framework
{
    public static class AutoMapperConfig
    {
        public static IMapper GetMapper()
            => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductDto>();
                cfg.CreateMap<User, UserDto>();
               // cfg.CreateMap<Domain.Profile, ProfileDto>();
            }).CreateMapper();
    }
}