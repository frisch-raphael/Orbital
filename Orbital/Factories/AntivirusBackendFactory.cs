using System;
using System.Collections.Generic;
using System.Text;
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
            switch (supportedAntivirus)
            {
                case SupportedAntivirus.Clamav: return new ClamavBackend();
                case SupportedAntivirus.TestAntivir: return new TestAntivirBackend();
                case SupportedAntivirus.McAfee: return new McAfeeBackend();
                case SupportedAntivirus.Comodo: return new ComodoBackend();
                default:
                    throw new ArgumentException("Trying to create unsupported antivirus backend");
            }
        }

    }
}
