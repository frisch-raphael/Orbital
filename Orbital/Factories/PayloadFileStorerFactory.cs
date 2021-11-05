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

        private readonly ILogger<PayloadFileStorer> Logger;
        private readonly HammerWrapper HammerWrapper;
        private readonly OrbitalContext OrbitalContext;
        private readonly IFunctionService FunctionService;

        public PayloadFileStorerFactory(
            ILogger<PayloadFileStorer> logger,
            HammerWrapper hammerWrapper,
            IFunctionService functionService, OrbitalContext orbitalContext)
        {
            Logger = logger;
            HammerWrapper = hammerWrapper;
            FunctionService = functionService;
            OrbitalContext = orbitalContext;
        }

        public IPayloadFileStorer Create(UploadedFile uploaded)
        {

            return new PayloadFileStorer(Logger, HammerWrapper, uploaded, FunctionService, OrbitalContext);
        }
    }
}
