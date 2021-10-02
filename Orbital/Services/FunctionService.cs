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
                AdressSection = marshalledFunction.adress_section,
                AdressOffset = marshalledFunction.adress_offset,
                Length = marshalledFunction.length
            };
        }

    }
}

//public string Name { get; set; }
//public string File { get; set; }
//public int FirstLine { get; set; }
//public System.UInt32 AdressSection { get; set; }
//public System.UInt32 AdressOffset { get; set; }
//public long Length { get; set; }
//public string Library { get; set; }
