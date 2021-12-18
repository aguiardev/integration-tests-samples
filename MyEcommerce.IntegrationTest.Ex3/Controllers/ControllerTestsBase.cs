using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class ControllerTestsBase
    {
        public ITestOutputHelper Output { get; private set; }

        public LoggerFactory LoggerFactory { get; private set; }

        public ControllerTestsBase(ITestOutputHelper output)
        {
            Output = output;
            LoggerFactory = new LoggerFactory();
        }
    }
}