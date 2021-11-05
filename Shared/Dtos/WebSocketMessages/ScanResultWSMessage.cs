using Shared.Dtos;
using Shared.Enums;

namespace Shared.ControllerResponses.Dtos
{
    public class ScanResultWsMessage
    {
        public Scan Scan { get; set; }
        public Payload Payload { get; set; }

    }
}
