using System;

namespace Shared.Config
{
    public class SharedOptionsRoot
    {
        public SharedOptions SharedOptions { get; set; }
    }

    public class RodinOptions : DomainOptions
    {
    }

    public class HammerOptions : DomainOptions
    {
    }

    public class DetonatorOptions : DomainOptions
    {
    }

    public class SharedOptions
    {
        public const string SharedKey = "SharedOptions";
        public RodinOptions rodinOptions { get; set; }
        public DetonatorOptions detonatorOptions { get; set; }
        public HammerOptions hammerOptions { get; set; }
    }


    public abstract class DomainOptions
    {
        public string Domain { get; set; }
        public int Port { get; set; }
        public bool Https { get; set; }
        public Uri BaseAddress
        {
            get
            {
                string protocol = Https ? "https" : "http";
                return new Uri($"{protocol}://{Domain}:{Port}", UriKind.Absolute);
            }
        }
    }
}
