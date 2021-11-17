using Microsoft.Extensions.Logging;
using Orbital.Model;
using Orbital.Services;
using Shared.Dtos;

namespace Orbital.Factories
{
    public interface IFunctionsDissecterFactory
    {
        ILogger<FunctionsDissecter> Logger { get; }
        IFunctionsDissecter Create(BackendPayload payload);
    }

    public class FunctionsDissecterFactory : IFunctionsDissecterFactory
    {
        public ILogger<FunctionsDissecter> Logger { get; }

        public FunctionsDissecterFactory(ILogger<FunctionsDissecter> logger)
        {
            Logger = logger;
        }

        public IFunctionsDissecter Create(BackendPayload payload)
        {
            return new FunctionsDissecter(Logger, payload);
        }
    }
}