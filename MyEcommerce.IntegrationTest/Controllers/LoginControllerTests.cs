using Microsoft.AspNetCore.Mvc.Testing;
using MyEcommerce.Api;
using MyEcommerce.Api.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class LoginControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _httpClient;
        private readonly ITestOutputHelper _output;

        public LoginControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public void Authenticate_UsingValidUserAndPassword_ReturnSucess()
        {
            var user = new User
            {
                Username = "batman",
                Password = "batman"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json"
            );

            var httpClientRequest = _httpClient.PostAsync("api/login", content)
                .GetAwaiter()
                .GetResult();

            Assert.Equal(HttpStatusCode.OK, httpClientRequest.StatusCode);
        }
    }
}