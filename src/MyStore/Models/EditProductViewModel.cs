﻿using Microsoft.AspNetCore.Mvc.Rendering;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Models
{
    public class EditProductViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Missing name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(1, 100000)]
        //[DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public List<SelectListItem> Categories { get; set; } =
            new List<SelectListItem>
            {
                new SelectListItem { Text = "Electronics", Value = "Electronics"},
                new SelectListItem { Text = "Tools", Value = "Tools"},
                new SelectListItem { Text = "Cars", Value = "Cars"}
            };

        public List<FileDto> Files { get; set; }

        public string Description { get; set; }
    }
}
