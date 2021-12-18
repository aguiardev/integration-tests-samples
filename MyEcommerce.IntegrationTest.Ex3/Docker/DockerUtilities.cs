using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyEcommerce.IntegrationTest.Docker
{
    [ExcludeFromCodeCoverage]
    public static class DockerUtilities
    {
        public static bool IsRunningOnWindows() => Environment.OSVersion.Platform == PlatformID.Win32NT;

        public static string GetFreePort()
        {
            // From https://stackoverflow.com/a/150974/4190785
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            var port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return port.ToString();
        }

        public static async Task EnsureDockerVolumesRemovedAsync(string volumeName)
            => await GetDockerClient().Volumes.RemoveAsync(volumeName);

        public static DockerClient GetDockerClient()
        {
            var dockerUri = IsRunningOnWindows()
                ? "npipe://./pipe/docker_engine"
                : "unix:///var/run/docker.sock";

            return new DockerClientConfiguration(new Uri(dockerUri))
                .CreateClient();
        }

        public static async Task<IList<ContainerListResponse>> ListContainersAsync(this DockerClient dockerClient)
            => await dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters() { All = true });

        public static async Task CreateVolumAsync(this DockerClient dockerClient, string volumeName)
        {
            var volumeList = await dockerClient.Volumes.ListAsync();
            if (!volumeList.Volumes.Any(v => v.Name.ToLower() == volumeName.ToLower()))
            {
                await dockerClient.Volumes.CreateAsync(new VolumesCreateParameters
                {
                    Name = volumeName,
                });
            }
        }

        public static async Task CreateImageAsync(this DockerClient dockerClient, string fromImagem)
            => await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
            {
                FromImage = fromImagem
            }, null, new Progress<JSONMessage>());

        public static async Task<ContainerListResponse> GetContainer(this DockerClient dockerClient, string containerName)
        {
            var containerList = await dockerClient.ListContainersAsync();

            var container = containerList
                .FirstOrDefault(c => c.Names.Any(n => n.Contains(containerName)));

            return container;
        }

        public static async Task EnsureDockerContainersStoppedAndRemovedAsync(this DockerClient dockerClient, string dockerContainerId)
        {
            await dockerClient.Containers
                .StopContainerAsync(dockerContainerId, new ContainerStopParameters());

            await dockerClient.Containers
                .RemoveContainerAsync(dockerContainerId, new ContainerRemoveParameters());
        }

        public static async Task CleanupRunningVolumesAsync(this DockerClient dockerClient, string dbContainerName, int hoursTillExpiration = -24)
        {
            var runningVolumes = await dockerClient.Volumes.ListAsync();

            foreach (var runningVolume in runningVolumes.Volumes.Where(v => v.Name == dbContainerName))
            {
                // Stopping all test volumes that are older than 24 hours
                var expiration = hoursTillExpiration > 0
                    ? hoursTillExpiration * -1
                    : hoursTillExpiration;

                if (DateTime.Parse(runningVolume.CreatedAt) < DateTime.UtcNow.AddHours(expiration))
                {
                    try
                    {
                        await EnsureDockerVolumesRemovedAsync(runningVolume.Name);
                    }
                    catch
                    {
                        // Ignoring failures to stop running containers
                    }
                }
            }
        }

        public static async Task CleanupRunningContainersAsync(this DockerClient dockerClient, string dbContainerName, int hoursTillExpiration = -24)
        {
            var runningContainers = await dockerClient.Containers
                .ListContainersAsync(new ContainersListParameters());

            foreach (var runningContainer in runningContainers.Where(cont => cont.Names.Any(n => n.Contains(dbContainerName))))
            {
                // Stopping all test containers that are older than 24 hours
                var expiration = hoursTillExpiration > 0
                    ? hoursTillExpiration * -1
                    : hoursTillExpiration;

                if (runningContainer.Created < DateTime.UtcNow.AddHours(expiration))
                {
                    try
                    {
                        await dockerClient.EnsureDockerContainersStoppedAndRemovedAsync(runningContainer.ID);
                    }
                    catch
                    {
                        // Ignoring failures to stop running containers
                    }
                }
            }
        }

        public static async Task<(string containerId, string port)> CreateAndStartContainerAsync(
            this DockerClient dockerClient, string containerName, string imageName, string volumeName, IList<string> environment, HostConfig hostConfig)
        {
            var port = 0;
            var containerId = "";
            var container = await dockerClient.GetContainer(containerName);

            await dockerClient.CleanupRunningContainersAsync(containerName);
            await dockerClient.CleanupRunningVolumesAsync(containerName);

            if (container is null) // create container, if one doesn't already exist
            {
                // This call ensures that the latest SQL Server Docker image is pulled
                await dockerClient.CreateImageAsync(imageName);

                // create a volume, if one doesn't already exist
                await dockerClient.CreateVolumAsync(volumeName);

                var newContainer = await dockerClient
                    .Containers
                    .CreateContainerAsync(new CreateContainerParameters
                    {
                        Name = containerName,
                        Image = imageName,
                        Env = environment,
                        HostConfig = hostConfig,
                    });

                containerId = newContainer.ID;
            }
            //else if (container.State.ToLower() == "exited") // Check container stoped
            //{
            //    containerId = container.ID;
            //}
            else
            {
                containerId = container.ID;
            }

            await dockerClient.Containers
                .StartContainerAsync(containerId, new ContainerStartParameters());

            container = await dockerClient.GetContainer(containerName);

            port = container.Ports.FirstOrDefault(f => f.Type == "tcp")?.PublicPort ?? 0;

            return (container.ID, port.ToString());
        }
    }
}