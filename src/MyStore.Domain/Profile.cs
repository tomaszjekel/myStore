using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class Profile
    {
        public Guid ProfileId { get; private set; }
        public Guid UserId { get; private set; }
        public List<Product> Products { get; private set; }

        public Profile()
        {

        }
        public Profile(Guid profileId, Guid userId, List<Product> products)
        {
            ProfileId = profileId;
            UserId = userId;
            Products = products;
        }
    }
}
