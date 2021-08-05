using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using MyEcommerce.Api;
using MyEcommerce.Api.Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class CustomerControllerTests
    {
        private const string _url = "api/customer";
        protected readonly WebApplicationFactory<Startup> _factory;
        protected readonly HttpClient _httpClient;
        protected readonly ITestOutputHelper _output;

        public CustomerControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _output = output;
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

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var httpClientRequest = await _httpClient.PostAsync(_url, content);
            _output.WriteLine(await httpClientRequest.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.Created, httpClientRequest.StatusCode);
        }
    }
}