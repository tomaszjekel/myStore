using MyStore.Services.DTO;
using System;
using System.Collections.Generic;

namespace MyStore.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<FileDto> Files { get; set; }
    }
}