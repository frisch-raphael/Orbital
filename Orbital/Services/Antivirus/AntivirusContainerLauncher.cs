using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;

namespace Orbital.Services.Antivirus
{
    public interface IAntivirusContainerLauncher
    {
        public Task<IList<ContainerListResponse>> PrepareContainers(int numberOfInstances);
    }

    public class AntivirusContainerLauncher : IAntivirusContainerLauncher
    {
        private DockerClient DockerClient { get; }
        private ILogger<AntivirusContainerLauncher> Logger { get; }
        private string ParentImageName { get; set; }

        public AntivirusContainerLauncher(
            DockerClientConfiguration dockerClientConf, ILogger<AntivirusContainerLauncher> logger, string parentImageName)
        {
            DockerClient = dockerClientConf.CreateClient();
            Logger = logger;
            ParentImageName = parentImageName;
        }

        public async Task<IList<ContainerListResponse>> PrepareContainers(int numberOfInstancesRequested)
        {

            Logger.LogInformation($"{numberOfInstancesRequested} container(s) for {ParentImageName} requested");

            var alreadyExistingContainers = await GetContainersAssociatedToImageName();

            var numberOfAlreadyExistingContainers = alreadyExistingContainers.Count();
            Logger.LogInformation($"{numberOfAlreadyExistingContainers} container(s) for {ParentImageName} already exist");


            if (numberOfAlreadyExistingContainers > numberOfInstancesRequested)
            {
                var excessContainerNumber = numberOfAlreadyExistingContainers - numberOfInstancesRequested;
                await DeleteN(excessContainerNumber);
            }
            else
            {
                var numberOfContainerMissing = numberOfInstancesRequested - numberOfAlreadyExistingContainers;
                await CreateN(numberOfContainerMissing);
            }

            return await StartContainers();
        }


        private async Task<IList<ContainerListResponse>> StartContainers()
        {

            var containersToLaunch = await GetContainersAssociatedToImageName();
            Parallel.ForEach(containersToLaunch, async container =>
            {
                var success = await DockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            });
            return containersToLaunch;
        }

        private async Task<IList<ContainerListResponse>> GetContainersAssociatedToImageName()
        {
            return await DockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    // what the hell is this way of filtering docker
                    All = true,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {
                            "ancestor", new Dictionary<string, bool>()
                            {
                                { ParentImageName, true },
                            }
                        }
                    }
                });
        }

        private string GenerateRandomName()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        private async Task CreateN(int numberOfContainerToCreate)
        {
            Logger.LogInformation($"Creating {numberOfContainerToCreate} container(s)");

            for (int i = 0; i < numberOfContainerToCreate; i++)
            {
                CreateContainerResponse response = await DockerClient.Containers.CreateContainerAsync(
                        new CreateContainerParameters
                        {
                            Image = ParentImageName,
                            Name = $"{ParentImageName}_{GenerateRandomName()}",
                            AttachStderr = true,
                            AttachStdout = true,
                            Tty = true
                        });

                Logger.LogInformation(ObjectDumper.Dump(response));
            }
        }

        private async Task DeleteN(int numberOfContainerToDelete)
        {
            Logger.LogInformation($"Deleting {numberOfContainerToDelete} container(s)");

            var containersToDelete = await DockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true,
                    Limit = numberOfContainerToDelete,
                    Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {
                            "ancestor", new Dictionary<string, bool>()
                            {
                                { ParentImageName, true },
                            }
                        }
                    }
                });

            Parallel.ForEach(containersToDelete, async container =>
            {
                await DockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters());
            });
        }

    }
}
