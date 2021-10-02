//using Microsoft.AspNetCore.Mvc;
//using Rodin.Services;
//using Shared.Dtos;
//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using static Rodin.Services.HammerWrapper;

//namespace Rodin.Controllers
//{


//    [ApiController]
//    [Route("api/bstr")]
//    public class TestBstr : ControllerBase
//    {

//        [HttpGet]
//        public string Get()
//        {
//            StringHammerResponse response = TestBstr();
//            return response.is_error ? response.error : response.data;
//        }

//    }

//    [ApiController]
//    [Route("api/bstres")]
//    public class TestBstrArray : ControllerBase
//    {

//        [HttpGet]
//        public string[] Get()
//        {
//            StringsHammerResponse response = TestBstrArray();
//            return response.is_error ? new string[] { response.error } : response.data;
//        }

//    }

//    [ApiController]
//    [Route("api/ints")]
//    public class TestIntegerArray : ControllerBase
//    {

//        [HttpGet]
//        public int[] Get()
//        {
//            IntPtr IntegerArrayReceiver = IntPtr.Zero;
//            int iSizeReceiver = 0;

//            TestInts(out IntegerArrayReceiver, out iSizeReceiver);

//            int[] pIntArray = new int[iSizeReceiver];

//            Marshal.Copy(IntegerArrayReceiver, pIntArray, 0, iSizeReceiver);

//            Marshal.FreeCoTaskMem(IntegerArrayReceiver);

//            return pIntArray;
//        }

//    }

//    [ApiController]
//    [Route("api/Functions")]
//    public class TestFunctions : ControllerBase
//    {

//        [HttpGet]
//        public MarshalledFunction[] Get()
//        {

//            IntPtr structsPointer;

//            int size;

//            TestFunctions(out structsPointer, out size);

//            MarshalledFunction[] Functions = new MarshalledFunction[size];

//            int FunctionSize = Marshal.SizeOf(typeof(MarshalledFunction));

//            for (int i = 0; i < size; i++)
//            {
//                Functions[i] = (MarshalledFunction)Marshal.PtrToStructure(structsPointer, typeof(MarshalledFunction));
//                structsPointer = IntPtr.Add(structsPointer, FunctionSize);
//            }

//            return Functions;
//        }

//    }

//    [ApiController]
//    [Route("api/Functionsresponse")]
//    public class TestFunctionsResponse : ControllerBase
//    {

//        [HttpGet]
//        public MarshalledFunction[] Get()
//        {

//            int size;

//            HammerResponse response = TestFunctionsResponse(out size);

//            if (response.is_error) throw new Exception(response.error);

//            MarshalledFunction[] Functions = new MarshalledFunction[size];

//            int FunctionSize = Marshal.SizeOf(typeof(MarshalledFunction));

//            for (int i = 0; i < size; i++)
//            {
//                Functions[i] = (MarshalledFunction)Marshal.PtrToStructure(response.pData, typeof(MarshalledFunction));
//                response.pData = IntPtr.Add(response.pData, FunctionSize);
//            }

//            return Functions;
//        }

//    }

//    //[ApiController]
//    //[Route("api/functions")]
//    //public class GetFunctions : ControllerBase
//    //{

//    //    [HttpGet]
//    //    public MarshalledFunction[] Get()
//    //    {

//    //        int size;

//    //        HammerResponse response = GetFunctions(out size);

//    //        if (response.is_error) throw new Exception(response.error);

//    //        MarshalledFunction[] Functions = new MarshalledFunction[size];

//    //        int FunctionSize = Marshal.SizeOf(typeof(MarshalledFunction));

//    //        for (int i = 0; i < size; i++)
//    //        {
//    //            Functions[i] = (MarshalledFunction)Marshal.PtrToStructure(response.pData, typeof(MarshalledFunction));
//    //            response.pData = IntPtr.Add(response.pData, FunctionSize);
//    //        }

//    //        return Functions;
//    //    }

//    //}

//    [ApiController]
//    [Route("api/sourcefiles")]
//    public class GetSourceFiles : ControllerBase
//    {

//        [HttpGet]
//        public SourceFile[] Get()
//        {

//            int size;

//            HammerResponse response = GetSourceFiles(out size);

//            if (response.is_error) throw new Exception(response.error);

//            SourceFile[] pdbSourceFiles = new SourceFile[size];

//            int pdbSourceFileSize = Marshal.SizeOf(typeof(SourceFile));

//            for (int i = 0; i < size; i++)
//            {
//                pdbSourceFiles[i] = (SourceFile)Marshal.PtrToStructure(response.pData, typeof(SourceFile));
//                response.pData = IntPtr.Add(response.pData, pdbSourceFileSize);
//            }

//            return pdbSourceFiles;
//        }

//    }
//}
