using System;
using System.Collections.Generic;
using System.Text;

namespace MyStore.Domain
{
    public class Addresses
    {
        public Guid Id { get; set; }
        public string FistName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string OrderNotes { get; set; }
    }
}
