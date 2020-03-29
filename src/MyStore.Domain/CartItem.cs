using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Domain
{
    public class CartItem
    {
        public int Id { get; set; }
        public Guid ProductId { get;  set; }
        public string ProductName { get;  set; }
        public int Quantity { get;  set; }
        public decimal UnitPrice { get;  set; }
        public decimal TotalPrice { get; set; }
        public int CartId { get; set; }
        public virtual Product Product { get; set; }
    }
}