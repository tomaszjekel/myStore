using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class ProductVariant
    {
        public Guid Id { get; set; }
        public Guid? ProductId{ get; set; } 
        public Guid? ColorId { get; set; }
        public Guid? SizeId { get; set; }
        public string Remarks { get; set; }
        public decimal? Price { get; set; }
        public bool? Isactive { get; set; }
        public Guid? UserId { get; set; }

    }
}
