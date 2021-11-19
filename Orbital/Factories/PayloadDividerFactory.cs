using Microsoft.Extensions.Logging;
using Orbital.Model;
using Orbital.Services;
using Shared.Dtos;

namespace Orbital.Factories
{
    public interface IPayloadDividerFactory
    {
        ILogger<PeDivider> Logger { get; }
        IPeDivider Create(BackendPayload payload);
    }

    public class PayloadDividerFactory : IPayloadDividerFactory
    {
        public ILogger<PeDivider> Logger { get; }

        public PayloadDividerFactory(ILogger<PeDivider> logger)
        {
            Logger = logger;
        }

        public IPeDivider Create(BackendPayload payload)
        {
            return new PeDivider(Logger, payload);
        }
    }
}