using DbUp;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.Api.Repositories;
using MyEcommerce.IntegrationTest.Docker;
using Respawn;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MyEcommerce.IntegrationTest.Fixture
{
    [ExcludeFromCodeCoverage]
    public class DatabaseFixture : IAsyncLifetime
    {
        private string _dockerPort;
        private static Checkpoint _checkpoint;
        private readonly DockerMsSqlDatabase _dockerMsSqlDatabase;
        
        public string DatabaseName { get; private set; }

        public MyEcommerceContext DbContext { get; private set; }

        public DatabaseFixture()
        {
            DatabaseName = "MyEcommerce";
            _dockerMsSqlDatabase = new DockerMsSqlDatabase();
        }

        public async Task InitializeAsync()
        {
            (_, _dockerPort) = await _dockerMsSqlDatabase.EnsureDockerStartedAndGetContainerIdAndPortAsync();

            _checkpoint = new Checkpoint
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" },
                WithReseed = true
            };
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public void ResetState()
            => _checkpoint
                .Reset(_dockerMsSqlDatabase.GetSqlConnectionString(_dockerPort, DatabaseName))
                .Wait();

        private MyEcommerceContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<MyEcommerceContext>()
                .UseSqlServer(_dockerMsSqlDatabase.GetSqlConnectionString(_dockerPort, DatabaseName))
                .Options;

            DbContext = new MyEcommerceContext(options);
            return DbContext;
        }

        public MyEcommerceContext Seed()
        {
            var conn = _dockerMsSqlDatabase.GetSqlConnectionString(_dockerPort, DatabaseName);

            // Note: If you want your application to create the database for you, add the following line
            EnsureDatabase.For.SqlDatabase(conn);

            var upgrader = DeployChanges.To
                .SqlDatabase(conn)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {

            }

            return CreateDbContext();
        }
    }
}