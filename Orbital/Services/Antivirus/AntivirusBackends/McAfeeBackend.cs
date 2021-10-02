using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orbital.Services.AntivirusBackends
{
    namespace Orbital.Services.AntivirusBackends
    {
        public class McAfeeBackend : AntivirusBackend
        {
            public override string Cmd { get; } = "/usr/local/uvscan/uvscan";

            public override string PayloadPathArg { get; } = "--SECURE";

            public override Regex OutputParser { get; } = new Regex("Found the");
        }
    }

}
