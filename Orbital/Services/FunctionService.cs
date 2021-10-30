using System.Runtime.InteropServices.ComTypes;
using Shared.Dtos;

namespace Orbital.Services
{
    public interface IFunctionService
    {
        Function CreateFunctionFromMarshalled(MarshalledFunction marshalledFunction, string pathToPayload);
    }

    public class FunctionService : IFunctionService
    {

        private readonly IPeFunctionOffsetGetter PeFunctionOffsetGetter;

        public FunctionService(IPeFunctionOffsetGetter peFunctionOffsetGetter)
        {
            PeFunctionOffsetGetter = peFunctionOffsetGetter;
        }

        public Function CreateFunctionFromMarshalled(MarshalledFunction marshalledFunction, string pathToPayload)
        {
            return new Function
            {
                Name = marshalledFunction.name,
                File = marshalledFunction.file,
                FirstLine = marshalledFunction.first_line,
                Offset = PeFunctionOffsetGetter.GetOffsetInPe(marshalledFunction.virtual_adress, pathToPayload),
                Length = marshalledFunction.length
            };
        }

    }
}

