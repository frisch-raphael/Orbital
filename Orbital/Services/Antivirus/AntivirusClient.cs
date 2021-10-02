﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Orbital.Factories;
using Orbital.Services.Antivirus;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Services.Antivirus
{

    public interface IAntivirusClient
    {

    }

    public class AntivirusClient : IAntivirusClient
    {

        private readonly IAntivirusImageBuilder ImageBuilder;
        private readonly IAntivirusContainerLauncher ContainerLauncher;
        private readonly IScannerService Scanner;
        private readonly ILogger<AntivirusClient> Logger;
        private SupportedAntivirus Antivirus { get; set; }

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


        public async Task<List<ScanResult>> Scan(string[] payloadsFileName)
        {
            Logger.LogInformation($"Setup for {Antivirus} started");
            await ImageBuilder.Build(Antivirus);
            var containers = await ContainerLauncher.prepareContainers(1);
            return await Scanner.LaunchScans(payloadsFileName, containers);
        }
    }
}

