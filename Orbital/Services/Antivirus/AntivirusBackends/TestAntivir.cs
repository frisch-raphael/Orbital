using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orbital.Services.AntivirusBackends
{
    namespace Orbital.Services.AntivirusBackends
    {
        public class TestAntivirBackend : AntivirusBackend
        {
            public override string BinaryPath { get; } = "echo";

            public override string PayloadPathArg { get; } = "orbital";
            public override List<string> OtherArgs { get; } = new List<string>();

            public override Regex OutputParser { get; } = new Regex("orbital");
        }
    }

}
