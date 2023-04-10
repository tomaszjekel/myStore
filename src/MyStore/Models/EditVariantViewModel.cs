using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace MyStore.Models
{
    public class EditVariantViewModel
    {
        public Guid? Id { get; set; }
        public Guid? ProductId { get; set; }
        public string VariantName { get; set; }
        public Guid? VariantColorId { get; set; }
        public Guid? VariantSizeId { get; set;}
        public Decimal? VariantPrice { get; set; }  
        public int? VariantQuantity { get; set; }
        public IEnumerable<SelectListItem> Colors { get; set; }
        public IEnumerable<SelectListItem> Sizes { get; set; }
        public int? Order { get; set; }

    }
}
