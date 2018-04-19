using System;

namespace MyStore.Domain
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        private OrderItem()
        {
        }

        public OrderItem(CartItem item)
        {
            Id = Guid.NewGuid();
            ProductId = item.ProductId;
            ProductName = item.ProductName;
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
        }
    }
}