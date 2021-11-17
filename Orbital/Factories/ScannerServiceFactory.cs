using Docker.DotNet;
using Shared.Enums;
using Orbital.Services.AntivirusBackends;
using Microsoft.Extensions.Logging;
using Orbital.Services.Antivirus;

namespace Orbital.Factories
{
    public interface IScannerServiceFactory
    {
        IScannerService Create(SupportedAntivirus supportedAntivirus);
    }

    public class ScannerServiceFactory : IScannerServiceFactory
    {
        private DockerClientConfiguration DockerClientConf { get; set; }
        public ILogger<ScannerService> Logger { get; }

        private readonly IAntivirusBackendFactory AntivirusBackendFactory;

        public ScannerServiceFactory(
            DockerClientConfiguration dockerClientConf, ILogger<ScannerService> logger, IAntivirusBackendFactory antivirusBackendFactory)
        {
            DockerClientConf = dockerClientConf;
            Logger = logger;
            this.AntivirusBackendFactory = antivirusBackendFactory;
        }

        public IScannerService Create(SupportedAntivirus supportedAntivirus)
        {
            return new ScannerService(
                DockerClientConf,
                Logger,
                supportedAntivirus,
                AntivirusBackendFactory);
        }
    }
}
