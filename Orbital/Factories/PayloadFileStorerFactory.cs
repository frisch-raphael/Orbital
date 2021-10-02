using Microsoft.Extensions.Logging;
using Orbital.Controllers;
using Orbital.Model;
using Orbital.Services;

namespace Orbital.Factories
{
    public interface IPayloadFileStorerFactory
    {
        IPayloadFileStorer Create(UploadedFile uploaded);
    }

    public class PayloadFileStorerFactory : IPayloadFileStorerFactory
    {

        private OrbitalContext RodinContext { get; }
        private ILogger<PayloadsController> Logger { get; }
        private HammerWrapper HammerWrapper { get; }
        public IFunctionService FunctionService { get; }

        public PayloadFileStorerFactory(
            ILogger<PayloadsController> logger,
            OrbitalContext rodinContext,
            HammerWrapper hammerWrapper,
            IFunctionService functionService)
        {
            Logger = logger;
            RodinContext = rodinContext;
            HammerWrapper = hammerWrapper;
            FunctionService = functionService;
        }

        public IPayloadFileStorer Create(UploadedFile uploaded)
        {

            return new PayloadFileStorer(Logger, RodinContext, HammerWrapper, uploaded);
        }
    }
}
