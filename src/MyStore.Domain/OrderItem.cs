using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Domain
{
    public class OrderItem
    {
        public Guid Id { get;  set; }
        public Guid ProductId { get;  set; }
        public string ProductName { get;  set; }
        public int Quantity { get;  set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitPrice { get;  set; }
        public decimal? TotalPrice => Quantity * UnitPrice;
        public string Size { get; set; }
        public Guid? SizeId { get; set; }

    }
}