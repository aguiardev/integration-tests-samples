using Bogus;
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
            var ids = 1;
            var customersFake = new Faker<Customer>(Constants.LOCALE_FAKER)
                .RuleFor(p => p.Id, () => ids++)
                .RuleFor(p => p.Name, faker => faker.Person.FirstName)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(p => p.Birth, faker => faker.Date.Between(
                    new DateTime(1950, 1, 1), new DateTime(2002, 12, 31)))
                .Generate(1000);

            Context.Customers.AddRange(customersFake);
            Context.SaveChanges();
        }
    }
}