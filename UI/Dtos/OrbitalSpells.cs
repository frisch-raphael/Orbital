using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ui.Dtos
{
    public enum SpellType
    {
        Analysis,
        Obfuscation
    }

    public class OrbitalSpells
    {
        public readonly string Title;

        public OrbitalSpells(string title, string spellUrl, string description, string image, SpellType spellType)
        {
            Title = title;
            SpellUrl = spellUrl;
            Description = description;
            Image = image;
            SpellType = spellType;
        }

        public readonly string SpellUrl;
        public readonly string Description;
        public readonly string Image;
        public readonly SpellType SpellType;
    }
}
