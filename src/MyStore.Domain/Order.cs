using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MyStore.Domain
{
    public class Order
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;  set; }
        public decimal TotalPrice { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public IEnumerable<OrderItem> Items { get;  set; }
        public Addresses Address { get; set; }
        public bool Completed { get; set; }

        

        //private Order()
        //{
        //}

        //public Order(Cart cart)
        //{
        //    Id = Guid.NewGuid();
        //    UserId = cart.UserId;
        //    Items = cart.Items.Select(i => new OrderItem(i));
        //    TotalPrice = cart.TotalPrice;
        //    CreatedAt = DateTime.UtcNow;
        //}
    }
}