
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orbital.Controllers;
using Orbital.Model;
using Orbital.Static;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Services
{
    public interface IPayloadFileStorer
    {
        Payload StorePayloadDataInDb();
        Task StoreFile();
        void RollStorageBack();
    }

    public class PayloadFileStorer : FileStorer, IPayloadFileStorer
    {
        public ILogger<PayloadsController> Logger { get; }
        private OrbitalContext OrbitalContext { get; }
        private HammerWrapper HammerWrapper { get; }

        //private readonly Dictionary<PayloadType, Func<string, List<Function>>> GetFunctions =
        //    new Dictionary<PayloadType, Func<string, List<Function>>>();


        public PayloadFileStorer(
            ILogger<PayloadsController> logger,
            OrbitalContext rodinContext,
            HammerWrapper hammerWrapper,
            UploadedFile uploadedFile) : base(logger, uploadedFile)
        {
            Logger = logger;
            OrbitalContext = rodinContext;
            HammerWrapper = hammerWrapper;
            //GetFunctions[PayloadType.AssemblyExecutable] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.AssemblyLibrary] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.NativeExecutable] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.NativeLibrary] = hammerWrapper.FetchFunctionsFromPdb;
            //GetFunctions[PayloadType.Other] = hammerWrapper.FetchFunctionsFromPdb;
        }


        public Payload StorePayloadDataInDb()
        {
            var payloadType = GetPayloadType();
            var functions = GetFunctions(payloadType);

            var payload = new Payload()
            {
                FileName = UploadedFile.TrustedFileName,
                StoragePath = LocalPathes.UploadDirectory + UploadedFile.StorageFileName,
                Functions = functions,
                Hash = GetHash(UploadedFile.File.OpenReadStream()),
                PayloadType = GetPayloadType()
            };

            OrbitalContext.Payloads.Add(payload);
            OrbitalContext.SaveChanges();
            return payload;

        }

        private List<Function> GetFunctions(PayloadType payloadType)
        {
            // for now functions from all payload types are extracted from pdb
            if (payloadType != PayloadType.Other)
            {
                return HammerWrapper.FetchFunctionsFromPdb(UploadedFile.StorageFullPath);
            }
            return new List<Function>();
        }

        private string GetHash(Stream fileStream)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = fileStream)
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "");
                }
            }
        }

        private PayloadType GetPayloadType()
        {
            switch (UploadedFile.Extension)
            {
                case (".dll"):
                    return PayloadType.NativeLibrary;
                case (".exe"):
                    return PayloadType.NativeExecutable;
                default:
                    return PayloadType.Other;
            }

        }

    }
}
