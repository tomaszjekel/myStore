using MyStore.Domain;
using System;

namespace MyStore.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public Guid UserId { get; set; }
        public int Quantity { get; set; }

    }
}