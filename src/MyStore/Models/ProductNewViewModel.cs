﻿using MyStore.Domain;
using MyStore.Services;
using MyStore.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStore.Models
{
    public class  ProductNewViewModel
    {
        public   IList<ProductViewModel> Products { get; set; }
        public  bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int? PageIndex { get; set; }
        public int TotalPages { get; set; }


    }
}
