using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Orbital.Pocos;
using Shared.Dtos;

namespace Orbital.Services
{
    public class PayloadDivider
    {
        private readonly BackendPayload BackendPayload;
        private readonly ILogger<PayloadDivider> Logger;

        public PayloadDivider( ILogger<PayloadDivider> logger, BackendPayload backendPayload)
        {
            Logger = logger;
            BackendPayload = backendPayload;
        }

        public void Divide()
        {
            foreach (var function in BackendPayload.Functions)
            {
                Logger.LogInformation("Starting to divide payload {PayloadName}", BackendPayload.FileName);
                ExtractAndStoreSubPayload(function);
            }
        }

        private void ExtractAndStoreSubPayload(Function function)
        {
            var subPayloadStorageFullPath = GetStorageFullPath(function);
            Directory.CreateDirectory(Path.GetDirectoryName(subPayloadStorageFullPath) ?? throw new InvalidOperationException());
            File.Copy(BackendPayload.StoragePath,subPayloadStorageFullPath);
            using var stream = new FileStream(subPayloadStorageFullPath, FileMode.Open, FileAccess.ReadWrite);
            CleanBeforeFunction(stream, function);

        }

        private static void CleanBeforeFunction(Stream fs, Function function)
        {
            fs.Position = 0;
            using var binaryWriter = new BinaryWriter(fs);

            binaryWriter.Write(Enumerable.Repeat((byte)0x00, (int)function.AdressOffset).ToArray());

        }

        private string GetStorageFullPath(Function function)
        {
            var subPayloadPath = Path.ChangeExtension(BackendPayload.StoragePath, null);
            var subPayloadStorageFileName = Regex.Replace(function.Name, "[^A-Za-z0-9 -]", string.Empty);
            return $"{subPayloadPath}/{subPayloadStorageFileName}";
        }

    }
}