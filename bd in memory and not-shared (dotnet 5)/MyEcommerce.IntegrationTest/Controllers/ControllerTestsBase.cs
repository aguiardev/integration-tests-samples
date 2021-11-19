using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEcommerce.Api.Entities;
using MyEcommerce.Api.Repositories;
using System;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class ControllerTestsBase : IDisposable
    {
        public ITestOutputHelper Output { get; private set; }

        public LoggerFactory LoggerFactory { get; private set; }

        public string DatabaseName { get; private set; }
        
        public MyEcommerceContext Context { get; private set; }

        public ControllerTestsBase(ITestOutputHelper output)
        {
            DatabaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MyEcommerceContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;

            Context = new MyEcommerceContext(options);
            Output = output;
            LoggerFactory = new LoggerFactory();

            Seed();
        }

        public void Dispose() => Context.Customers.RemoveRange(Context.Customers);

        public void Seed()
        {
            var ids = 1;
            var customersFake = new Faker<Customer>(Constants.LOCALE_FAKER)
                .RuleFor(p => p.Id, () => ids++)
                .RuleFor(p => p.Name, faker => faker.Person.FirstName)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(p => p.Birth, faker => faker.Date.Between(
                    new DateTime(1950, 1, 1), new DateTime(2002, 12, 31)))
                .Generate(10);

            Context.Customers.AddRange(customersFake);
            Context.SaveChanges();
        }
    }
}