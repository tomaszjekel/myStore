using MyStore.Domain;
using System;
using System.Collections.Generic;

namespace MyStore.Services.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<FilesUpload> Files { get; set; }
        public int CityId { get; set; }

    }
}