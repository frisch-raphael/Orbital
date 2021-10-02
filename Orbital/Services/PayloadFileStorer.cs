
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

        public PayloadFileStorer(
            ILogger<PayloadsController> logger,
            OrbitalContext rodinContext,
            HammerWrapper hammerWrapper,
            UploadedFile uploadedFile) : base(logger, uploadedFile)
        {
            Logger = logger;
            OrbitalContext = rodinContext;
            HammerWrapper = hammerWrapper;
        }


        public Payload StorePayloadDataInDb()
        {
            var payload = new Payload()
            {
                FileName = UploadedFile.TrustedFileName,
                StoragePath = LocalPathes.UploadDirectory + UploadedFile.StorageFileName,
                //Functions = HammerWrapper.FetchFunctionsFromPdb(UploadedFile.StorageFullPath),
                Functions = new List<Function>(),
                Hash = GetHash(UploadedFile.File.OpenReadStream()),
                PayloadType = GetPayloadType()
            };

            OrbitalContext.Payloads.Add(payload);
            OrbitalContext.SaveChanges();
            return payload;

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
                    return PayloadType.kNativeLibrary;
                case (".exe"):
                    return PayloadType.kNativeExecutable;
                default:
                    throw new ArgumentException("Payload type not recognized");
            }

        }

    }
}
