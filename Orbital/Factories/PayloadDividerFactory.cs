using Microsoft.Extensions.Logging;
using Orbital.Model;
using Orbital.Services;
using Shared.Dtos;

namespace Orbital.Factories
{
    public interface IPayloadDividerFactory
    {
        ILogger<PayloadDivider> Logger { get; }
        IPayloadDivider Create(BackendPayload payload);
    }

    public class PayloadDividerFactory : IPayloadDividerFactory
    {
        public ILogger<PayloadDivider> Logger { get; }

        public PayloadDividerFactory(ILogger<PayloadDivider> logger)
        {
            Logger = logger;
        }

        public IPayloadDivider Create(BackendPayload payload)
        {
            return new PayloadDivider(Logger, payload);
        }
    }
}