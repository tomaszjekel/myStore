using MyStore.Domain;
using MyStore.Services;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;

namespace MyStore.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductUserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<FilesUpload> Files { get; set; }
        public string City { get; set; }
        public int Quantity { get; set; }
    }
}