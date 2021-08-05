using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class ControllerTestsBase : IClassFixture<WebApplicationFactoryBase>
    {
        public string Url { get; set; }
        private const string _url = "api/customer";
        private readonly ITestOutputHelper _output;
        private readonly WebApplicationFactoryBase _factory;
        private dynamic _token;
        protected HttpClient _client;

        public ControllerTestsBase(WebApplicationFactoryBase factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _client = factory.CreateClient();
            _token = new ExpandoObject();
            _token.sub = Guid.NewGuid();
        }

        public void SetRoles(dynamic roles) => _token.role = roles;

        public void WriteLineOutput(string content) => _output.WriteLine(content);

        public void UseToken() => _client.SetFakeBearerToken((object)_token);
    }
}