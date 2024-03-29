﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyStore.Domain;
using MyStore.Services.DTO;

namespace MyStore.Models
{
    public class CreateProductViewModel
    {
        //[RegularExpression(@".\S+.", ErrorMessage = "No white space allowed")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        
        [Required]
        public string Category { get; set; }
        
        [Required]
        [Range(1, 100000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public int? Quantity { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
            //new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "Blonde", Value = "Blonde"},
            //    new SelectListItem { Text = "Red", Value = "Red"},
            //    new SelectListItem { Text = "Black", Value = "Black"}
            //};


        public IEnumerable<SelectListItem> Cities { get; set; }
        public string SelectedCity { get; set; }

        public List<FileDto> Files { get; set; }

        public string Description { get; set; }


        public string VariantName { get; set; }
        public IEnumerable<SelectListItem> Colors { get; set; }
        public IEnumerable<SelectListItem> Sizes { get;set; }
        public Guid VariantColorId { get; set; }
        public Guid VariantSizeId { get; set; }
        public decimal VariantPrice { get; set; }
        public int? VariantQuantity { get; set; }
        public List<ProductVariant> Variants { get; set; }

        public string DefaultImage { get; set; }

        public int? Order { get; set; }
    }
}