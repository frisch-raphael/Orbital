using Shared.Enums;

namespace Shared.Dtos
{
    public class ScanResult
    {
        public int Id { get; set; }
        public int PayloadId { get; set; }
        public bool IsDone { get; set; }
        public bool IsError { get; set; }
        public bool IsFlagged { get; set; }
        public SupportedAntivirus Antivirus { get; set; }
        public System.DateTime ScanDate { get; set; }
    }
}
