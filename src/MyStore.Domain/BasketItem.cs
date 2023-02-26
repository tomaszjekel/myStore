using System;

namespace MyStore.Domain
{
    public class BasketItem
    {
        public int Id { get; set; }
        public Item Item { get; set; }
        public Guid UserId { get; set; }
    }
}
