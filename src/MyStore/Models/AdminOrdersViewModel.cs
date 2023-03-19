using MyStore.Domain;
using System.Collections.Generic;

namespace MyStore.Models
{
    public class AdminOrdersViewModel
    {
        public List<Order> orders;
        public int count;
        public int pageIndex;
    }
}
