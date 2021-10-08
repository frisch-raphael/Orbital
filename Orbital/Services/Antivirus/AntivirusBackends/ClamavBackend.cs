using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shared.Enums;

namespace Orbital.Services.AntivirusBackends
{
    public class ClamavBackend : AntivirusBackend
    {
        public override string BinaryPath { get; } = "clamscan";

        public override string PayloadPathArg { get; } = "--no-summary";

        public override Regex OutputParser { get; } = new Regex("FOUND");
    }
}
