using Microsoft.Extensions.Logging;
using Orbital.Controllers;
using Orbital.Model;
using Orbital.Pocos;
using Orbital.Services;

namespace Orbital.Factories
{
    public interface IPayloadFileStorerFactory
    {
        IPayloadFileStorer Create(UploadedFile uploaded);
    }

    public class PayloadFileStorerFactory : IPayloadFileStorerFactory
    {

        private OrbitalContext OrbitalContext { get; }
        private ILogger<PayloadFileStorer> Logger { get; }
        private HammerWrapper HammerWrapper { get; }
        public IFunctionService FunctionService { get; }

        public PayloadFileStorerFactory(
            ILogger<PayloadFileStorer> logger,
            OrbitalContext orbitalContext,
            HammerWrapper hammerWrapper,
            IFunctionService functionService)
        {
            Logger = logger;
            OrbitalContext = orbitalContext;
            HammerWrapper = hammerWrapper;
            FunctionService = functionService;
        }

        public IPayloadFileStorer Create(UploadedFile uploaded)
        {

            return new PayloadFileStorer(Logger, OrbitalContext, HammerWrapper, uploaded, FunctionService);
        }
    }
}
