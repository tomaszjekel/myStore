using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MyStore.Domain
{
    public class Cart
    {
        public int Id { get; set; }
       
        public Guid UserId { get; set; }
        public IEnumerable<CartItem> Items;

        public decimal TotalPrice { get; set; }

    }
}