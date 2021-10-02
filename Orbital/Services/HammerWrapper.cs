using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using Shared.Dtos;

namespace Orbital.Services
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringHammerResponse
    {

        [MarshalAs(UnmanagedType.BStr)]
        public string data;
        [MarshalAs(UnmanagedType.BStr)]
        public string error;
        [MarshalAs(UnmanagedType.I1)]
        public bool is_error;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct StringsHammerResponse
    {

        [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
        public string[] data;
        [MarshalAs(UnmanagedType.BStr)]
        public string error;
        [MarshalAs(UnmanagedType.I1)]
        public bool is_error;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct HammerResponse
    {
        public System.IntPtr pData;
        [MarshalAs(UnmanagedType.BStr)]
        public string error;
        [MarshalAs(UnmanagedType.I1)]
        public bool is_error;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class SourceFile
    {
        [MarshalAs(UnmanagedType.BStr)]
        public string file_name;

    }


    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class MarshalledFunction
    {
        [MarshalAs(UnmanagedType.BStr), StringLength(100)]
        public string name;
        [MarshalAs(UnmanagedType.BStr), StringLength(300)]
        public string file;
        public int first_line;
        public System.UInt32 adress_section;
        public System.UInt32 adress_offset;
        public long length;
    };

    public class HammerWrapper
    {
        private const string HammerDllPath = "Hammer.dll";

        public IFunctionService FunctionService { get; }

        public HammerWrapper(IFunctionService functionService)
        {
            FunctionService = functionService;
        }

        [DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern HammerResponse GetFunctions([Out] out int sizeReceiver, [MarshalAs(UnmanagedType.BStr)] string pePath);

        [DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern HammerResponse GetSourceFiles([Out] out int sizeReceiver);

        public List<Function> FetchFunctionsFromPdb(string pathToPdbSource)
        {

            int size;

            HammerResponse response = GetFunctions(out size, pathToPdbSource);

            if (response.is_error) throw new Exception(response.error);

            MarshalledFunction[] marshalledFunctions = new MarshalledFunction[size];

            int FunctionSize = Marshal.SizeOf(typeof(MarshalledFunction));

            for (int i = 0; i < size; i++)
            {
                marshalledFunctions[i] = (MarshalledFunction)Marshal.PtrToStructure(response.pData, typeof(MarshalledFunction));
                response.pData = IntPtr.Add(response.pData, FunctionSize);
            }

            List<Function> functions = marshalledFunctions.Select(marshalledFunction =>
                FunctionService.CreateFunctionFromMarshalled(marshalledFunction)).ToList();


            return functions;
        }

        //[DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        //private static extern StringHammerResponse TestBstr();

        //[DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        //private static extern StringsHammerResponse TestBstrArray();

        //[DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        //private static extern void TestInts([Out] out System.IntPtr IntegerArrayReceiver, [Out] out int sizeReceiver);

        //[DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        //private static extern void TestFunctions([Out] out System.IntPtr FunctionsReceiver, [Out] out int sizeReceiver);

        //[DllImport(HammerDllPath, CallingConvention = CallingConvention.Cdecl)]
        //private static extern HammerResponse TestFunctionsResponse([Out] out int sizeReceiver);

        //
    }
}
