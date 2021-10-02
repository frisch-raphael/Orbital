using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enums;

namespace Rodin.Services
{
    public class ScannerService
    {
        private readonly Dictionary<SupportedAntivirus, string> cmdLines =
            new Dictionary<SupportedAntivirus, string>() {
                { SupportedAntivirus.Defender, "Test" },
                { SupportedAntivirus.Clamav, "Test2" },
            };

        public void Scan()
        {

        }

    }
}
