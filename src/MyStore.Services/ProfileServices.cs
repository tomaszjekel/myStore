using AutoMapper;
using MyStore.Domain.Repositories;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public class ProfileServices: IProfileServices
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public ProfileServices(IProfileRepository profileRepository,
                IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<ProfileDto> GetAsync(Guid id)
        {
            var profile = await _profileRepository.GetAsync(id);

            return _mapper.Map<ProfileDto>(profile);
        }

        public async Task  CreateAsync(Guid id)
        {
        }
    }
}
