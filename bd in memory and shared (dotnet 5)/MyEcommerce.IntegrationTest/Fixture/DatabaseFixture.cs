using Microsoft.EntityFrameworkCore;
using MyEcommerce.Api.Entities;
using MyEcommerce.Api.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MyEcommerce.IntegrationTest.Fixture
{
    [ExcludeFromCodeCoverage]
    public class DatabaseFixture
    {
        public string DatabaseName { get; private set; }
        public MyEcommerceContext Context { get; private set; }

        public DatabaseFixture()
        {
            DatabaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MyEcommerceContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;

            Context = new MyEcommerceContext(options);
        }

        public void Reset()
            => Context.Customers.RemoveRange(Context.Customers);
       
        public void Seed()
        {
            Context.Customers.AddRange(
                new Customer(1, "Tiago", new DateTime(2000, 01, 05), "tiago@email.com"),
                new Customer(2, "André", new DateTime(1999, 05, 04), "andre@email.com"),
                new Customer(3, "Nathaly", new DateTime(1995, 07, 18), "nathaly@email.com"));

            Context.SaveChanges();
        }
    }
}