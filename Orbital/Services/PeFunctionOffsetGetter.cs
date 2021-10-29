using System.Linq;
using AsmResolver.PE;
using AsmResolver.PE.File;

namespace Orbital.Services
{
    public interface IPeFunctionOffsetGetter
    {
        long GetOffset(long rva, string pathToPe);
    }

    public class PeFunctionOffsetGetter : IPeFunctionOffsetGetter
    {
        public long GetOffset(long rva, string pathToPe)
        {
            var peFile = PEFile.FromFile(pathToPe);
            var peSection = peFile.Sections.First(s => s.Name == ".text");
            return rva - peSection.Rva + (long)peSection.Offset;
        }
    }
}