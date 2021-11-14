using System;

namespace MyEcommerce.Api.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public string Email { get; set; }

        public Customer()
        {

        }

        public Customer(int id, string name, DateTime birth, string email)
        {
            Id = id;
            Name = name;
            Birth = birth;
            Email = email;
        }
    }
}
