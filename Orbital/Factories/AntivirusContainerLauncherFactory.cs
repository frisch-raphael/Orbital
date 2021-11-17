using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orbital.Services;
using Docker.DotNet;
using Shared.Enums;
using Orbital.Services.AntivirusBackends;
using Microsoft.Extensions.Logging;
using Orbital.Services.Antivirus;

namespace Orbital.Factories
{
    public interface IAntivirusContainerLauncherFactory
    {
        IAntivirusContainerLauncher Create(SupportedAntivirus supportedAntivirus);
    }

    public class AntivirusContainerLauncherFactory : IAntivirusContainerLauncherFactory
    {
        private DockerClientConfiguration DockerClientConf { get; set; }
        public ILogger<AntivirusContainerLauncher> Logger { get; }

        public AntivirusContainerLauncherFactory(
            DockerClientConfiguration dockerClientConf, ILogger<AntivirusContainerLauncher> logger)
        {
            DockerClientConf = dockerClientConf;
            Logger = logger;
        }

        public IAntivirusContainerLauncher Create(SupportedAntivirus supportedAntivirus)
        {
            return new AntivirusContainerLauncher(
                DockerClientConf,
                Logger,
                supportedAntivirus.ToString().ToLower());
        }
    }
}
