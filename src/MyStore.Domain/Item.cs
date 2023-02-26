using MyStore.Domain;
using System;

namespace MyStore.Domain
{
    public class Item
    {
        public int Id { get; set; }
        public Product Product { get; set; }    
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
