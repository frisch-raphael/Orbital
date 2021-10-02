using Docker.DotNet;
using Shared.Enums;
using Orbital.Services.AntivirusBackends;
using Microsoft.Extensions.Logging;
using Orbital.Services.Antivirus;

namespace Orbital.Factories
{
    public interface IScannerServiceFactory
    {
        ScannerService Create(SupportedAntivirus supportedAntivirus);
    }

    public class ScannerServiceFactory : IScannerServiceFactory
    {
        private DockerClientConfiguration DockerClientConf { get; set; }
        public ILogger<ScannerService> Logger { get; }

        private readonly IAntivirusBackendFactory antivirusBackendFactory;

        public ScannerServiceFactory(
            DockerClientConfiguration dockerClientConf, ILogger<ScannerService> logger, IAntivirusBackendFactory antivirusBackendFactory)
        {
            DockerClientConf = dockerClientConf;
            Logger = logger;
            this.antivirusBackendFactory = antivirusBackendFactory;
        }

        public ScannerService Create(SupportedAntivirus supportedAntivirus)
        {
            return new ScannerService(
                DockerClientConf,
                Logger,
                supportedAntivirus,
                antivirusBackendFactory);
        }
    }
}
