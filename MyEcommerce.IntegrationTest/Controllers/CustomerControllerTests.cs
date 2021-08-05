using AutoBogus;
using MyEcommerce.Api.Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class CustomerControllerTests : ControllerTestsBase
    {
        public CustomerControllerTests(WebApplicationFactoryBase factory, ITestOutputHelper output) : base(factory, output)
        {
            Url = "api/customer";
            SetRoles(new[] { "manager", "employee" });
        }

        [Fact(DisplayName = "Create Using Valid Customer Return Created")]
        public async Task Create_UsingValidCustomer_ReturnCreated()
        {
            var customer = new AutoFaker<Customer>()
                .RuleFor(p => p.Id, () => 0)
                .RuleFor(p => p.Name, faker => faker.Person.FirstName)
                .RuleFor(p => p.Email, faker => faker.Person.Email)
                .RuleFor(p => p.Birth, faker => faker.Date.Between(
                    new DateTime(1970, 1, 1),
                    new DateTime(2000, 12, 31)))
                .Generate();

            var content = new StringContent(
                JsonConvert.SerializeObject(customer),
                Encoding.UTF8,
                "application/json"
            );

            UseToken();

            var httpClientRequest = await _client.PostAsync(Url, content);

            WriteLineOutput(await httpClientRequest.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, httpClientRequest.StatusCode);
        }
    }
}