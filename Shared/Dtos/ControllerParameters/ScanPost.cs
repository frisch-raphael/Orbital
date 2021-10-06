using System.Collections.Generic;
using Shared.Enums;

namespace Shared.Dtos
{

    public class ScanPost
    {
        public List<SupportedAntivirus> Antiviruses;
        public int PayloadId;
    }

}
