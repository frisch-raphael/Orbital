using Shared.Enums;

namespace Shared.ControllerResponses.Dtos
{
    public class ScanControllerResponse
    {
        public bool IsFlagged { get; set; }
        public List<SupportedAntivirus> Antivirus { get; set; }

    }
}
