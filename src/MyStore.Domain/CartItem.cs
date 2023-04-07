using System;

namespace MyStore.Domain
{
    public class CartItem
    {
        public Guid ProductId { get;   set; }
        public string ProductName { get;  set; }
        public string Img { get;  set; }
        public int Quantity { get;  set; }
        public decimal? UnitPrice { get;  set; }
        public decimal? TotalPrice => Quantity * UnitPrice;
        public string Size { get; set; }
        public Guid? SizeId { get; set; }

        public void IncreaseQuantity()
            => Quantity++;
    }
}