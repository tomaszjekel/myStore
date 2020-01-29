using System;
using System.Collections.Generic;

namespace MyStore.Domain
{
    public class Product
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;set ;} 
        public string Name { get;  set; }
        public string Category { get;  set; }
        public string Description { get;  set; }
        public decimal Price { get;  set; }
        public List<FilesUpload> Files { get; set; }



        private Product()
        {
        }

        public Product(Guid userId , string name, string category, 
            decimal price, string description): this(userId, new Guid(),
            name, category, price, description)
        {
        }

        public Product(Guid id, Guid userId, string name,
            string category, decimal price, string description)
        {
            Id = id;
            UserId = userId;
            SetName(name);
            Category = category;
            SetPrice(price);
            Description = description;
            
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is empty.",   
                    nameof(name));
            }
            if (name.Length > 50)
            {
                throw new ArgumentException("Product name is too long.",
                    nameof(name));
            }
            Name = name;
        }

        public void SetPrice(decimal price)
        {
            if (price < 1 || price > 100000)
            {
                throw new ArgumentException("Invalid product price.",
                    nameof(price));
            }
            Price = price;
        }
    }
}