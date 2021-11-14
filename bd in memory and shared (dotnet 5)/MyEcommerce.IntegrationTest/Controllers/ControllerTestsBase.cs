using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MyEcommerce.IntegrationTest.Controllers
{
    public class ControllerTestsBase
    {
        public const string LOCALE_FAKER = "pt_BR";
        public ITestOutputHelper Output;
        
        public LoggerFactory LoggerFactory = new();

        public ControllerTestsBase(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}