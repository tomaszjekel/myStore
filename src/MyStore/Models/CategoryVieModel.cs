using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Domain.Repositories;

namespace MyStore.Models
{
    public class CategoryVieModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Category> Categories {get; set;}
    }
}
