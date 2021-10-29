
using System;
using System.ComponentModel.DataAnnotations.Schema;
using KellermanSoftware.CompareNetObjects;

namespace Shared.Dtos
{
    public class Function : IEquatable<Function>
    {
        public int Id { get; set; }
        public int BackendPayloadId { get; set; }
        public string Name { get ;set; }
        public string File { get; set; }
        /// <summary>
        /// The line at which the function begins
        /// </summary>
        public int FirstLine { get; set; }
        /// <summary>
        /// For PEs, the offset in the file to the function (in bytes).
        /// </summary>
        public long Offset { get; set; }
        /// <summary>
        /// For PEs, the length of the function (in bytes).
        /// </summary>
        public long Length { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Function other)
        {
            var compareLogic = new CompareLogic();
            var result = compareLogic.Compare(this, other);
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
