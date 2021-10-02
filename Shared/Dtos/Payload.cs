using System.Collections.Generic;
using Shared.ControllerResponses.Dtos;
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
        public string StoragePath { get; set; }
        public PayloadType PayloadType { get; set; }

    }


}
