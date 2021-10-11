using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orbital.Services.AntivirusBackends
{
    public interface IAntivirusBackend
    {
        public string BinaryPath { get; }
        public string PayloadPathArg { get; }
        public List<string> OtherArgs { get; }
        public Regex OutputParser { get; }
        public List<string> GetFullCmd(string PayloadFileName);
    }

    public abstract class AntivirusBackend : IAntivirusBackend
    {
        public abstract string BinaryPath { get; }
        public abstract string PayloadPathArg { get; }
        public virtual List<string> OtherArgs { get; } = new List<string>();

        public abstract Regex OutputParser { get; }

        public List<string> GetFullCmd(string PayloadFileName)
        {
            var FullCmd = new List<string>() { BinaryPath };
            FullCmd.AddRange(OtherArgs);
            FullCmd.Add(PayloadPathArg);
            FullCmd.Add("/" + PayloadFileName);
            return FullCmd;
        }
    }
}
