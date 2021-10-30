
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orbital.Controllers;
using Orbital.Model;
using Orbital.Pocos;
using Orbital.Static;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Services
{
    public interface IPayloadFileStorer
    {
        BackendPayload StorePayloadDataInDb();
        Task StoreFile();
        void RollStorageBack();
    }

    public class PayloadFileStorer : FileStorer, IPayloadFileStorer
    {
        public ILogger<PayloadFileStorer> Logger { get; }
        private OrbitalContext OrbitalContext { get; }
        private HammerWrapper HammerWrapper { get; }

        private readonly IFunctionService FunctionService;
        //private readonly Dictionary<PayloadType, Func<string, List<Function>>> GetFunctions =
        //    new Dictionary<PayloadType, Func<string, List<Function>>>();


        public PayloadFileStorer(
            ILogger<PayloadFileStorer> logger,
            HammerWrapper hammerWrapper,
            UploadedFile uploadedFile, IFunctionService functionService) : base(logger, uploadedFile)
        {
            Logger = logger;
            HammerWrapper = hammerWrapper;
            FunctionService = functionService;
            //GetFunctions[PayloadType.AssemblyExecutable] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.AssemblyLibrary] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.NativeExecutable] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.NativeLibrary] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.Other] = hammerWrapper.FetchFunctionsFromPdb;
        }


        public BackendPayload StorePayloadDataInDb()
        {
            var payloadType = GetPayloadType();
            var functions = GetFunctions(payloadType);

            var payload = new BackendPayload()
            {
                FileName = UploadedFile.TrustedFileName,
                StoragePath = LocalPathes.UploadDirectory + UploadedFile.StorageFileName,
                Functions = functions,
                Hash = GetHash(UploadedFile.File.OpenReadStream()),
                PayloadType = GetPayloadType()
            };

            OrbitalContext.BackendPayloads.Add(payload);
            OrbitalContext.SaveChanges();
            return payload;

        }

        private List<Function> GetFunctions(PayloadType payloadType)
        {
            // for now functions from all payload types are extracted from pdb
            if (payloadType == PayloadType.Other) return new List<Function>();
            var marshalledFunctions = HammerWrapper.FetchFunctionsFromPdb(UploadedFile.StorageFullPath);


            var functions = marshalledFunctions.Select(marshalledFunction =>
                FunctionService.CreateFunctionFromMarshalled(marshalledFunction, UploadedFile.StorageFullPath)).ToList();

            return functions;
        }

        private static string GetHash(Stream fileStream)
        {
            using var md5 = MD5.Create();
            using var stream = fileStream;
            var hashBytes = md5.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        private PayloadType GetPayloadType()
        {
            return UploadedFile.Extension switch
            {
                (".dll") => PayloadType.NativeLibrary,
                (".exe") => PayloadType.NativeExecutable,
                _ => PayloadType.Other
            };
        }

    }
}
