using MyStore.Domain;
using System.Collections.Generic;

namespace MyStore.Models
{
    public class CategoriesViemModel
    {
        public List<Category> Categories { get; set; }
        public string Name { get; set; }
    }
}
