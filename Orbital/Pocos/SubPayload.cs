using System.Collections.Generic;
using Shared.Dtos;

namespace Orbital.Pocos
{
    public class SubPayload
    {
        public int Id { get; set; }

        public int PayloadId { get; set; }

        public string StoragePath { get; set; }

        public List<Function> Functions { get; set; }
    }
}