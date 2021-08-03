using Microsoft.AspNetCore.Mvc.Testing;
using MyEcommerce.Api;
using Xunit;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public UsuarioControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Test()
        {

        }
    }
}