using System.Collections.Generic;
using System.Linq;
using AsmResolver.PE;
using AsmResolver.PE.File;
using static System.Net.Mime.MediaTypeNames;

namespace Orbital.Services
{
    internal interface IPeModifier
    {
        public void DeleteSections(List<string> sectionNamesToDelete);

        public void AddToSection(string sectionName);
        // public void DeleteSectionsExcept(List<string> sectionNamesNotToDelete);
    }

    public class PeModifier : IPeModifier

    {
        private IPEFile PeFile { get; }
        public PeModifier(string pePath)
        {
            PeFile = PEFile.FromFile(pePath);
        }

        public void DeleteSections(List<string> sectionNamesToDelete)
        {
            throw new System.NotImplementedException();
        }

        public void AddToSection(string sectionName)
{
            var peSection = PeFile.Sections.First(s => s.Name == sectionName);
            throw new System.NotImplementedException();
        }
    }
}