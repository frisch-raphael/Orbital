using Shared.Enums;

namespace Shared.Dtos
{

    public abstract class ScanResult
    {
        public bool IsDone { get; set; }
        public bool IsError { get; set; }
        public bool IsFlagged { get; set; }

    }

    public class ScanControllerResponse : ScanResult
    {
        public int Id { get; set; }
        public int PayloadId { get; set; }
        public SupportedAntivirus Antivirus { get; set; }
        public System.DateTime ScanDate { get; set; }
    }

    public class DissectControllerResponse 
    {
        public int PayloadId { get; set; }
        public SupportedAntivirus Antivirus { get; set; }
        public System.DateTime ScanDate { get; set; }

        public int SubPayloadId { get; set; }
    }
}
