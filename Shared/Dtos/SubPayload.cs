using System.Collections.Generic;
using Shared.Dtos;

namespace Shared.Pocos
{
    public class SubPayload
    {
        public int Id { get; set; }

        public string StorageFullPath { get; set; }

        public List<Function> Functions { get; set; }


    }
}