
using System;
using System.ComponentModel.DataAnnotations.Schema;
using KellermanSoftware.CompareNetObjects;

namespace Shared.Dtos
{
    public class Function : IEquatable<Function>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public int FirstLine { get; set; }
        public System.UInt32 AdressSection { get; set; }
        public System.UInt32 AdressOffset { get; set; }
        public long Length { get; set; }

        [ForeignKey("PayloadId")]
        public int PayloadId { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Function other)
        {
            CompareLogic compareLogic = new CompareLogic();
            ComparisonResult result = compareLogic.Compare(this, other);
            return result.AreEqual;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Name.GetHashCode();
            hash = (hash * 7) + File.GetHashCode();
            return hash;
        }



        //public Function(string name,
        //                string file,
        //                int firstLine,
        //                uint adressSection,
        //                uint adressOffset,
        //                long length,
        //                string library,
        //                int id)
        //{
        //    Name = name;
        //    File = file;
        //    FirstLine = firstLine;
        //    AdressSection = adressSection;
        //    AdressOffset = adressOffset;
        //    Length = length;
        //    Library = library;
        //    Id = id;
        //}


    }


}
