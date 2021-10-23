using System;
using Orbital.Services.AntivirusBackends.Orbital.Services.AntivirusBackends;
using Shared.Enums;

namespace Orbital.Services.AntivirusBackends
{
    public interface IAntivirusBackendFactory
    {
        IAntivirusBackend Create(SupportedAntivirus supportedAntivirus);
    }

    public class AntivirusBackendFactory : IAntivirusBackendFactory
    {
        public IAntivirusBackend Create(SupportedAntivirus supportedAntivirus)
        {
            return supportedAntivirus switch
            {
                SupportedAntivirus.Clamav => new ClamavBackend(),
                SupportedAntivirus.TestAntivir => new TestAntivirBackend(),
                SupportedAntivirus.McAfee => new McAfeeBackend(),
                SupportedAntivirus.Comodo => new ComodoBackend(),
                _ => throw new ArgumentException("Trying to create unsupported antivirus backend")
            };
        }

    }
}
