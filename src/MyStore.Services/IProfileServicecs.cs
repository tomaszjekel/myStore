using MyStore.Domain;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyStore.Services
{
    public interface IProfileServices
    {
        Task<ProfileDto> GetAsync(Guid id);
    }
}
