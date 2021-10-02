using Shared.Enums;

namespace Shared.ControllerResponses.Dtos
{
    public class ScanResponse
    {
        public bool IsFlagged { get; set; }
        public SupportedAntivirus Antivirus { get; set; }

    }
}
