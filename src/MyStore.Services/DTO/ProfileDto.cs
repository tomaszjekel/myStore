using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Services.DTO
{
    public class ProfileDto
    {
        public Guid ProfileId { get; set; }
        public Guid UserId { get; set; }
        public List<ProductDto> Products {get;set;}
    }
}
