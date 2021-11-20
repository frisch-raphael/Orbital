using System.Collections.Generic;
using Shared.Dtos;

namespace Orbital.Pocos
{

    public class DivideResult
    {
        /// <summary>
        /// The functions contained in the subPayload<br></br>
        /// For now one subPayload = one function
        /// </summary>
        public List<Function> Functions { get; set; }
        /// <summary>
        /// The path to the subPayload
        /// </summary>
        public string SubPayloadFullPath { get; set; }
    }
}