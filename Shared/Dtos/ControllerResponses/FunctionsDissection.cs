using System.Collections.Generic;
using Shared.Enums;
using Shared.Pocos;

namespace Shared.Dtos
{

    public class SubPayloadScanResult 
    {
        public int Id { get; set; }
        public List<SubPayloadScanResult> SubPayloadScanResultChildren {get; set;}
        public SubPayload SubPayload { get; set; }
        public OperationState ScanState { get; set; }
        public FlaggedState FlaggedState { get; set; }

    }


    public class FunctionsDissection
    {
        public int Id { get; set; }
        public int PayloadId { get; set; }
        public OperationState DissectionState { get; set; }
        public SupportedAntivirus Antivirus { get; set; }
        public System.DateTime ScanDate { get; set; }
        public List<SubPayloadScanResult> SubPayloadScanResultRoots {get; set;}
    }
}
