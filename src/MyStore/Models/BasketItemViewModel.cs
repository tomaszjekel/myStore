using System;

namespace MyStore.Models
{
    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public Guid UserId { get; set; }

        public ItemViewModel ItemViewModel { get; set; }
    }
}
