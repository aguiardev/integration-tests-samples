using AutoBogus;
using MyEcommerce.Api.Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    //[Collection(nameof(CustomerControllerTests))]
    [Collection("CustomerControllerTests")]
    public class CustomerControllerTests : ControllerTestsBase
    {
        public CustomerControllerTests(WebApplicationFactoryBase factory, ITestOutputHelper output) : base(factory, output)
        {
            Url = "api/customer";
            SetRoles(new[] { "manager", "employee" });
        }

        [Fact(DisplayName = "Create Using Valid Customer Return Created"), TestPriority(1)]
        public void Create_UsingValidCustomer_ReturnCreated()
        {
            WriteLineOutput(DateTime.Now.ToString("hh:mm:ss"));

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

            var httpClientRequest = _client.PostAsync(Url, content).GetAwaiter().GetResult();

            WriteLineOutput(httpClientRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.Equal(HttpStatusCode.Created, httpClientRequest.StatusCode);
        }

        [Fact(DisplayName = "Update Using Valid Customer Return No Content"), TestPriority(2)]
        public void Update_UsingValidCustomer_ReturnNoContent()
        {
            WriteLineOutput(DateTime.Now.ToString("hh:mm:ss"));

            var customer = new AutoFaker<Customer>()
                .RuleFor(p => p.Id, () => 1)
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

            var httpClientRequest = _client.PutAsync(Url + "/1", content).GetAwaiter().GetResult();

            WriteLineOutput(httpClientRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult());

            Assert.Equal(HttpStatusCode.NoContent, httpClientRequest.StatusCode);
        }
    }
}