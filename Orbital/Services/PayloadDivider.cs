using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Orbital.Model;
using Shared.Dtos;

namespace Orbital.Services
{
    public class PayloadDivider
    {
        private readonly Payload Payload;
        public ILogger<PayloadDivider> Logger { get; }

        public PayloadDivider( ILogger<PayloadDivider> logger, Payload payload)
        {
            Payload = payload;
        }

        public ICollection<Payload> Divide()
        {
            return new List<Payload>() { new Payload(){FileName = "test" } };
        }
    }
}