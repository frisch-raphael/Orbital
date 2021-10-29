using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
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

        public void Divide(List<int> idsOfFunctionsInScope)
{
            Logger.LogInformation("Starting to Divide {PayloadName}", BackendPayload.FileName);

            var functionsInScope = BackendPayload.Functions.Where(f => idsOfFunctionsInScope.Contains(f.Id)).ToList();


            Parallel.ForEachAsync(
                functionsInScope, 
                new ParallelOptions { MaxDegreeOfParallelism = 20 }, 
                (function, _) => ExtractAndStoreSubPayload(
                    function, 
                    new FileStream(BackendPayload.StoragePath, FileMode.Open, FileAccess.Read)));

  }

        private async ValueTask ExtractAndStoreSubPayload(Function function, Stream payloadStream)
        {
            Logger.LogInformation(
                "Extracting function {FunctionName} from {PayloadName}", 
                function.Name, BackendPayload.FileName);
            var subPayloadStorageFullPath = GetStorageFullPath(function);
            Directory.CreateDirectory(Path.GetDirectoryName(subPayloadStorageFullPath) ?? throw new InvalidOperationException());
            var functionBytes = await ExtractSubPayloadBytes(function, payloadStream);

            await using var subPayloadStream = new FileStream(subPayloadStorageFullPath, FileMode.Create, FileAccess.Write);
            await subPayloadStream.WriteAsync(functionBytes);
            // var bytes = Encoding.UTF8.GetBytes(content);
            // await subPayloadStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static async Task<byte[]> ExtractSubPayloadBytes(Function function, Stream payloadStream)
        {
            var functionBuffer = new byte[80000];
            // payloadStream.Position = function.VirtualAddress;
            await payloadStream.ReadAsync(functionBuffer);

            return functionBuffer;
        }

        private string GetStorageFullPath(Function function)
        {
            var subPayloadPath = Path.ChangeExtension(BackendPayload.StoragePath, null);
            var subPayloadStorageFileName = Regex.Replace(function.Name, "[^A-Za-z0-9 -]", string.Empty);
            return $"{subPayloadPath}/{subPayloadStorageFileName}.raw";
        }

    }
}