using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enums;

namespace Ui.Dtos
{
    public enum SpellType
    {
        Scan,
        Cloack
    }

    public class OrbitalSpell
    {
        public string Title { get; init; }
        public string Endpoint { get; init; }
        public string Description { get; init; }
        public string Image { get; init; }
        public List<PayloadType> SupportedPayloads { get; init; }

    }


    public class OrbitalScan : OrbitalSpell
    {
        public bool IsAppliedToAllAntivirus { get; init; }

    }
}
