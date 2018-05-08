using AutoMapper;
using MyStore.Domain;
using MyStore.Domain.Repositories;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public class FileService: IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FileService(IFileRepository fileRepository, IUserRepository userRepository,
                IMapper mapper)
        {
            _fileRepository = fileRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<FileDto> GetAsync(Guid id)
        {
            var file = await _fileRepository.GetAsync(id);

            return _mapper.Map<FileDto>(file);
        }

        public async Task<IEnumerable<FileDto>> BrowseAsync(Guid userId)
        {
            var file = await _fileRepository.BrowseAsync(userId);

            return file.Select(_mapper.Map<FileDto>);
        }

        public async Task<IEnumerable<FileDto>> BrowseByProductAsync(Guid userId, Guid productId)
        {
            var file = await _fileRepository.BrowseByProductAsync(userId, productId);

            return file.Select(_mapper.Map<FileDto>);
        }

        public async Task CreateAsync(Guid userId, Guid? productId, string name , DateTime data)
        {
            var fu = new FilesUpload(userId, productId, name, data);
            await _fileRepository.CreateAsync(fu);
        }
        public async Task UpdateAsync(Guid productId)
        {
            await _fileRepository.UpdateAsync(productId);
        }

    }
}
