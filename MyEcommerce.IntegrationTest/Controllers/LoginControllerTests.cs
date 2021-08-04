using Microsoft.AspNetCore.Mvc.Testing;
using MyEcommerce.Api;
using MyEcommerce.Api.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        [Fact(DisplayName = "Authenticate Using Valid User And Password Return Success")]
        public async Task Authenticate_UsingValidUserAndPassword_ReturnSuccess()
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

            var httpClientRequest = await _httpClient.PostAsync("api/login", content);

            Assert.Equal(HttpStatusCode.OK, httpClientRequest.StatusCode);
        }
    }
}