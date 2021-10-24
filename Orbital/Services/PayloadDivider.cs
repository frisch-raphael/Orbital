using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Orbital.Pocos;
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

        public ICollection<SubPayload> Divide()
        {
            return new List<SubPayload>() { new SubPayload(){FileName = "test", Payload = Payload } };
        }
    }
}