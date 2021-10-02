using Shared.Enums;
using Microsoft.Extensions.Logging;
using Orbital.Services.Antivirus;

namespace Orbital.Factories
{
    public interface IAntivirusClientFactory
    {
        AntivirusClient Create(SupportedAntivirus supportedAntivirus);
    }

    public class AntivirusClientFactory : IAntivirusClientFactory
    {
        private readonly IAntivirusImageBuilder ImageBuilder;
        private readonly IAntivirusContainerLauncherFactory ContainerLauncherFactory;
        private readonly ILogger<AntivirusClient> Logger;
        private readonly IScannerServiceFactory ScannerFactory;

        public AntivirusClientFactory(
            IAntivirusImageBuilder imageBuilder,
            IAntivirusContainerLauncherFactory containerLauncherFactory,
            ILogger<AntivirusClient> logger,
            IScannerServiceFactory scannerFactory)
        {
            ImageBuilder = imageBuilder;
            ContainerLauncherFactory = containerLauncherFactory;
            Logger = logger;
            ScannerFactory = scannerFactory;
        }

        public AntivirusClient Create(SupportedAntivirus supportedAntivirus)
        {
            return new AntivirusClient(
                ImageBuilder,
                ContainerLauncherFactory,
                ScannerFactory,
                Logger,
                supportedAntivirus);
        }
    }
}
