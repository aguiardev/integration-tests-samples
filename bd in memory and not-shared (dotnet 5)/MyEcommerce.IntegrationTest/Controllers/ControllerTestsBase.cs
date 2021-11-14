using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyEcommerce.Api.Repositories;
using System;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class ControllerTestsBase
    {
        public const string LOCALE_FAKER = "pt_BR";
        public string DatabaseName { get; private set; }
        public ITestOutputHelper Output { get; private set; }
        public MyEcommerceContext Context { get; private set; }
        public LoggerFactory LoggerFactory = new();

        public ControllerTestsBase(ITestOutputHelper output)
        {
            DatabaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<MyEcommerceContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;

            Context = new MyEcommerceContext(options);
            Output = output;
        }
    }
}