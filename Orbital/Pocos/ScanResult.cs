using Shared.Enums;

namespace Orbital.Pocos
{



    public class ScanResult
    {
        public string FilePath { get; set; }
        public FlaggedState FlaggedState { get; set; }
        public bool IsError { get; set; }
    }
}