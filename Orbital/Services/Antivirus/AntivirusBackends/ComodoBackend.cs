using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orbital.Services.AntivirusBackends
{
    namespace Orbital.Services.AntivirusBackends
    {
        public class ComodoBackend : AntivirusBackend
        {
            public override string BinaryPath { get; } = "/opt/COMODO/cmdscan";

            public override string PayloadPathArg { get; } = "-s";

            public override List<string> OtherArgs { get; } = new List<string> { "-v" };

            public override Regex OutputParser { get; } = new Regex("Found Virus, Malware Name is");
        }
    }

}
