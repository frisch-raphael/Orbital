using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ui.Dtos
{
    public enum SpellType
    {
        Scan,
        Cloack
    }

    public class OrbitalSpell
    {
        public readonly string Title;
        public readonly string Endpoint;
        public readonly string Description;
        public readonly string Image;
        public readonly SpellType SpellType;

        public OrbitalSpell(
            string title,
            string endpoint,
            string description,
            string image,
            SpellType spellType)
        {
            Title = title;
            Endpoint = endpoint;
            Description = description;
            Image = image;
            SpellType = spellType;
        }

    }


    public class OrbitalScan : OrbitalSpell
    {
        public readonly bool IsAppliedToAllAntivirus;

        public OrbitalScan(
            string title,
            string endpoint,
            string description,
            string image,
            SpellType spellType,
            bool isAppliedToAllAntivirus = false) : base(
                title,
                endpoint,
                description,
                image,
                spellType
                )
        {
            IsAppliedToAllAntivirus = isAppliedToAllAntivirus;
        }

    }
}
