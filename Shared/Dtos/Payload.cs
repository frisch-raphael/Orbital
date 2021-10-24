using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Dtos
{
    public class Payload
    {
        public int Id { get; set; }
        //ICollection<SourceFile> SourceFiles;
        public ICollection<Function> Functions { get; set; }
        public string Hash { get; set; }
        public string FileName { get; set; }
        public PayloadType PayloadType { get; set; }

        public Payload()
        {

        }

        public Payload(BackendPayload backendPayload)
        {
            Id = backendPayload.Id;
            FileName = backendPayload.FileName;
            Functions = backendPayload.Functions;
            Hash = backendPayload.Hash;
            PayloadType = backendPayload.PayloadType;

        }
    }


}
