using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orbital.Services.AntivirusBackends
{
    public interface IAntivirusBackend
    {
        public string Cmd { get; }
        public string PayloadPathArg { get; }
        public List<string> OtherArgs { get; }

        public Regex OutputParser { get; }
    }

    public abstract class AntivirusBackend : IAntivirusBackend
    {
        public abstract string Cmd { get; }
        public abstract string PayloadPathArg { get; }
        public virtual List<string> OtherArgs { get; } = new List<string>();

        public abstract Regex OutputParser { get; }
    }
}
