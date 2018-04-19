using System;

namespace MyStore.Domain
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Category { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }

        private Product()
        {
        }

        public Product(string name, string category, 
            decimal price): this(Guid.NewGuid(), 
            name, category, price)
        {
        }

        public Product(Guid id, string name,
            string category, decimal price)
        {
            Id = id;
            SetName(name);
            Category = category;
            SetPrice(price);
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