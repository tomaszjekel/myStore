﻿using System;

namespace MyStore.Domain
{
    public class OrderItem
    {
        public Guid Id { get;  set; }
        public Guid ProductId { get;  set; }
        public string ProductName { get;  set; }
        public int Quantity { get;  set; }
        public decimal UnitPrice { get;  set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public string Size { get; set; }
  

        
    }
}