using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Dtos
{

    public class DissectionPost
    {
        public int NumberOfDocker;
        public int PayloadId;
        public SupportedAntivirus SupportedAntivirus;
        public List<int> FunctionIds;
    }

}
