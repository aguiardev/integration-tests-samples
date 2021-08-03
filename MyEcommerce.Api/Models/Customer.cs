using System;

namespace MyEcommerce.Api.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public string Email { get; set; }
    }
}
