using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Dtos
{


    public class Scan
    {
        public int Id { get; set; }
        public int PayloadId { get; set; }
        public SupportedAntivirus Antivirus { get; set; }

        public OperationState OperationState { get; set; }

        public FlaggedState FlaggedState { get; set; }

        public System.DateTime ScanDate { get; set; }
    }


}
