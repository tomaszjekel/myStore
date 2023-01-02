using System;

namespace MyStore.Domain
{
    public class CartItem
    {
        public Guid ProductId { get;   set; }
        public string ProductName { get;  set; }
        public string Img { get;  set; }

        public int Quantity { get;  set; }
        public decimal UnitPrice { get;  set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        //public  CartItem(Product product)
        //{
        //    ProductId = product.Id;
        //    ProductName = product.Name;
        //    Quantity = 1;
        //    Img = product.Img;
        //    UnitPrice = product.Price;
        //}

        public void IncreaseQuantity()
            => Quantity++;
    }
}