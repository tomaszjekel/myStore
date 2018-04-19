using System;
using System.Collections.Generic;
using System.Linq;

namespace MyStore.Domain
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal TotalPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IEnumerable<OrderItem> Items { get; private set; }

        private Order()
        {
        }

        public Order(Cart cart)
        {
            Id = Guid.NewGuid();
            UserId = cart.UserId;
            Items = cart.Items.Select(i => new OrderItem(i));
            TotalPrice = cart.TotalPrice;
            CreatedAt = DateTime.UtcNow;
        }
    }
}