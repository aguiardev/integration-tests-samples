using Docker.DotNet.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MyEcommerce.IntegrationTest.Docker
{
    [ExcludeFromCodeCoverage]
    public class DockerMsSqlDatabase
    {
        public const string DB_PASSWORD = "yourStrong(!)Password";
        public const string DB_USER = "SA";
        public const string DB_IMAGE = "mcr.microsoft.com/mssql/server";
        public const string DB_IMAGE_TAG = "2017-latest";
        public const string DB_CONTAINER_NAME = "IntegrationTestingContainer_Accessioning";
        public const string DB_VOLUME_NAME = "IntegrationTestingVolume_Accessioning";

        public DockerMsSqlDatabase()
        {

        }

        private string GetImageName() => $"{DB_IMAGE}:{DB_IMAGE_TAG}";

        public string GetSqlConnectionString(string port)
            => $"Data Source=localhost,{port};Integrated Security=False;User ID={DB_USER};Password={DB_PASSWORD}";

        public string GetSqlConnectionString(string port, string databaseName)
            => $"Data Source=localhost,{port};Database={databaseName};Integrated Security=False;User ID={DB_USER};Password={DB_PASSWORD}";

        public async Task<(string containerId, string port)> EnsureDockerStartedAndGetContainerIdAndPortAsync()
        {
            var dockerClient = DockerUtilities.GetDockerClient();
            
            (string containerId, string port) = await dockerClient.CreateAndStartContainerAsync(
                DB_CONTAINER_NAME,
                GetImageName(),
                DB_VOLUME_NAME,
                GetEnvironment(),
                GetHostConfig());

            await WaitUntilDatabaseAvailableAsync(port);

            return (containerId, port);
        }

        private HostConfig GetHostConfig()
            => new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {
                        "1433/tcp",
                        new PortBinding[]
                        {
                            new PortBinding
                            {
                                HostPort = DockerUtilities.GetFreePort()
                            }
                        }
                    }
                },
                Binds = new List<string>
                {
                    $"{DB_VOLUME_NAME}:/Accessioning_data"
                }
            };

        private IList<string> GetEnvironment() => new List<string>
        {
            "ACCEPT_EULA=Y",
            $"SA_PASSWORD={DB_PASSWORD}"
        };

        private async Task WaitUntilDatabaseAvailableAsync(string databasePort)
        {
            var start = DateTime.UtcNow;
            const int maxWaitTimeSeconds = 60;
            var connectionEstablished = false;
            while (!connectionEstablished && start.AddSeconds(maxWaitTimeSeconds) > DateTime.UtcNow)
            {
                try
                {
                    var sqlConnectionString = GetSqlConnectionString(databasePort);
                    using var sqlConnection = new SqlConnection(sqlConnectionString);
                    await sqlConnection.OpenAsync();
                    connectionEstablished = true;
                }
                catch (Exception ex)
                {
                    // If opening the SQL connection fails, SQL Server is not ready yet
                    await Task.Delay(500);
                }
            }

            if (!connectionEstablished)
            {
                throw new Exception($"Connection to the SQL docker database could not be established within {maxWaitTimeSeconds} seconds.");
            }

            return;
        }
    }
}