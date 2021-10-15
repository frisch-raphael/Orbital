using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enums;
using Ui.Enums;

namespace Ui.Dtos
{


    public class OrbitalSpell
    {
        public string Title { get; init; }
        public string Endpoint { get; init; }
        public string Description { get; init; }
        public string Image { get; init; }
        public List<PayloadType> SupportedPayloads { get; init; }
        public List<SpellConfiguration> ConfigurationNeeded { get; init; } = new List<SpellConfiguration>();

    }


    public class OrbitalScan : OrbitalSpell
    {

    }
}
