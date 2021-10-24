using Shared.Dtos;

namespace Orbital.Pocos
{
    public class SubPayload : Payload
    {
        public Payload Payload { get; set; }
    }
}