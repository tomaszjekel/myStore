using System;
namespace MyStore.Domain.Repositories
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsSubCategory { get; set; }
    }
}
