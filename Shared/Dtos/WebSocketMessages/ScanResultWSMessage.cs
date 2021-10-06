using Shared.Dtos;
using Shared.Enums;

namespace Shared.ControllerResponses.Dtos
{
    public class ScanResultWSMessage
    {
        public ScanResult ScanResult { get; set; }
        public Payload Payload { get; set; }

    }
}
