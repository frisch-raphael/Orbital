using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Orbital.Factories;
using Orbital.Pocos;
using Orbital.Services.Antivirus;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Services.Antivirus
{

    public interface IAntivirusClient
    {
        Task<List<ScanResult>> ScanAsync(string[] payloadsFileName, int maxNumberOfDockerContainer = 10);
        SupportedAntivirus Antivirus { get; set; }
    }

    public class AntivirusClient : IAntivirusClient
    {

        private readonly IAntivirusImageBuilder ImageBuilder;
        private readonly IAntivirusContainerLauncher ContainerLauncher;
        private readonly IScannerService Scanner;
        private readonly ILogger<AntivirusClient> Logger;
        public SupportedAntivirus Antivirus { get; set; }

        public AntivirusClient(
            IAntivirusImageBuilder imageBuilder,
            IAntivirusContainerLauncherFactory containerLauncherFactory,
            IScannerServiceFactory scannerFactory,
            ILogger<AntivirusClient> logger,
            SupportedAntivirus supportedAntivirus)
        {
            ImageBuilder = imageBuilder;
            ContainerLauncher = containerLauncherFactory.Create(supportedAntivirus);
            Scanner = scannerFactory.Create(supportedAntivirus);
            Logger = logger;
            Antivirus = supportedAntivirus;
        }


        public async Task<List<ScanResult>> ScanAsync(string[] payloadsFileName, int maxNumberOfDockerContainer = 10)
        {
            Logger.LogInformation($"Setup for {Antivirus} started");
            await ImageBuilder.Build(Antivirus);
            var numDockers = GetNumberOfDocker(maxNumberOfDockerContainer, payloadsFileName.Length);
            var containers = await ContainerLauncher.PrepareContainers(numDockers);
            return await Scanner.LaunchScans(payloadsFileName, containers);
        }

        private static int GetNumberOfDocker(int maxNumberOfDockerContainer, int numberOfFileToScan)
        {
            return numberOfFileToScan >= maxNumberOfDockerContainer ? maxNumberOfDockerContainer : numberOfFileToScan;
        }
    }
}

