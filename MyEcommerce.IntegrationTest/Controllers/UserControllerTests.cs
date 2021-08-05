using Microsoft.AspNetCore.Mvc.Testing;
using MyEcommerce.Api;
using MyEcommerce.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        private const string _url = "api/user/login";
        protected readonly WebApplicationFactory<Startup> _factory;
        protected readonly HttpClient _httpClient;
        protected readonly ITestOutputHelper _output;

        public UserControllerTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _output = output;
        }

        public Task DisposeAsync()
        {
            _httpClient.Dispose();

            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await Authenticate_UsingValidUserAndPassword_ReturnSuccess();
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

            var httpClientRequest = await _httpClient.PostAsync(_url, content);

            var response = await httpClientRequest.Content.ReadAsStringAsync();
            //Token = GetToken(response);
            _output.WriteLine(response);

            Assert.Equal(HttpStatusCode.OK, httpClientRequest.StatusCode);
        }

        // private string GetToken(string response)
        // {
        //     var obj = JObject.Parse(response);

        //     return obj.Property("token").Value.ToString();
        // }

        [Fact(DisplayName = "Authenticate Using Invalid User Or Password Return Not Found")]
        public async Task Authenticate_UsingValidUserOrPassword_ReturnNotFound()
        {
            var user = new User
            {
                Username = "username",
                Password = "password"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json"
            );

            var httpClientRequest = await _httpClient.PostAsync(_url, content);
            _output.WriteLine(await httpClientRequest.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.NotFound, httpClientRequest.StatusCode);
        }
    }
}