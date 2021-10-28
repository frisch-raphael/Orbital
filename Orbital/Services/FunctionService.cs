using Shared.Dtos;

namespace Orbital.Services
{
    public interface IFunctionService
    {
        Function CreateFunctionFromMarshalled(MarshalledFunction marshalledFunction);
    }

    public class FunctionService : IFunctionService
    {
        public Function CreateFunctionFromMarshalled(MarshalledFunction marshalledFunction)
        {
            return new Function
            {
                Name = marshalledFunction.name,
                File = marshalledFunction.file,
                FirstLine = marshalledFunction.first_line,
                VirtualAddress = marshalledFunction.virtual_adress,
                Length = marshalledFunction.length
            };
        }

    }
}

