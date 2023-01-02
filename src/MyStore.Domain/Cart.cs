using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MyStore.Domain
{
    public class Cart
    {
        private readonly IDictionary<Guid, CartItem> _items = 
            new Dictionary<Guid, CartItem>();
        
        public Guid UserId { get; private set; }
        public IEnumerable<CartItem> Items => _items.Select(x => x.Value);
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);

        public Cart(User user)
        {
            UserId = user.Id;
        }

        public void AddProduct(Product product)
        {
            _items.TryGetValue(product.Id, out CartItem item);
            if (item == null)
            {
                //item = new CartItem(product);
                _items[product.Id] = item;

                return;
            }
            item.IncreaseQuantity();
        }
    }
}