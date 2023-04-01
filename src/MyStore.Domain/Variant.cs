using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class Variant
    {
        public Guid Id { get; set; }
        public string VarintName { get; set; }
        public string VarinatType { get; set; }
        public bool? IsActive { get; set; }

    }
}
