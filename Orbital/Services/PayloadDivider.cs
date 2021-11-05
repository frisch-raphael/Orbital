using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Orbital.Pocos;
using Orbital.Static;
using Shared.Dtos;

namespace Orbital.Services
{
    public class PayloadDivider
    {
        private readonly BackendPayload BackendPayload;
        private readonly ILogger<PayloadDivider> Logger;

        public PayloadDivider(ILogger<PayloadDivider> logger, BackendPayload backendPayload)
        {
            Logger = logger;
            BackendPayload = backendPayload;
        }

        private List<DivideResult> DivideResults { get; } = new();


        public async Task<List<DivideResult>> DivideInHalf()
        {
            var firstHalfPath = LocalPathes.UploadDirectory + "firstHalf.raw";
            var secondHalfPath = LocalPathes.UploadDirectory + "secondHalf.raw";

            var payloadStream = new FileStream(BackendPayload.StoragePath, FileMode.Open, FileAccess.Read);

            var firstHalfBytes = await ExtractBytesFromFile(payloadStream.Length / 2, 0, payloadStream);
            var secondHalfBytes = await ExtractBytesFromFile(payloadStream.Length / 2, payloadStream.Length / 2, payloadStream);

            await using var firstHalfPayloadStream =  new FileStream(firstHalfPath, FileMode.Create, FileAccess.Write);
            await using var secondHalfPayloadStream =  new FileStream(firstHalfPath, FileMode.Create, FileAccess.Write);

            await firstHalfPayloadStream.WriteAsync(firstHalfBytes);
            await secondHalfPayloadStream.WriteAsync(secondHalfBytes);

            DivideResults.Add(
                new DivideResult
                {
                    FunctionIds = new List<int> { },
                    SubPayloadFullPath = firstHalfPath
                });

            DivideResults.Add(
                new DivideResult
                {
                    FunctionIds = new List<int> { },
                    SubPayloadFullPath = secondHalfPath
                });

            return DivideResults;
        }


        public async Task<List<DivideResult>> Divide(List<int> idsOfFunctionsInScope)
        {
            Logger.LogInformation("Starting to Divide {PayloadName}", BackendPayload.FileName);

            var functionsInScope = BackendPayload.Functions.Where(f => idsOfFunctionsInScope.Contains(f.Id)).ToList();


            await Parallel.ForEachAsync(
                functionsInScope,
                new ParallelOptions { MaxDegreeOfParallelism = 20 },
                (function, _) => ExtractAndStoreSubPayload(
                    function,
                    new FileStream(BackendPayload.StoragePath, FileMode.Open, FileAccess.Read)));
            if (DivideResults.Count == 0)
                throw new InvalidOperationException("No subPayload path returned by payload divider");
            return DivideResults;
        }

        // side effect : add payload path of subpayload to SubPayloadPathes
        private async ValueTask ExtractAndStoreSubPayload(Function function, Stream payloadStream)
        {
            Logger.LogInformation(
                "Extracting function {FunctionName} from {PayloadName}",
                function.Name, BackendPayload.FileName);
            var subPayloadFullPath = GetStorageFullPath(function);
            Directory.CreateDirectory(Path.GetDirectoryName(subPayloadFullPath) ??
                                      throw new InvalidOperationException());
            var functionBytes = await ExtractBytesFromFile(function.Length, function.Offset, payloadStream);

            await using var subPayloadStream =
                new FileStream(subPayloadFullPath, FileMode.Create, FileAccess.Write);
            await subPayloadStream.WriteAsync(functionBytes);

            DivideResults.Add(
                new DivideResult
                {
                    FunctionIds = new List<int> { function.Id },
                    SubPayloadFullPath = subPayloadFullPath
                });
            // var bytes = Encoding.UTF8.GetBytes(content);
            // await subPayloadStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static async Task<byte[]> ExtractBytesFromFile(long length, long offset, Stream file)
        {
            var functionBuffer = new byte[length];
            file.Position = offset;
            await file.ReadAsync(functionBuffer);

            return functionBuffer;
        }

        private string GetStorageFullPath(Function function)
        {
            var subPayloadPath = Path.ChangeExtension(BackendPayload.StoragePath, null);
            var subPayloadStorageFileName = Regex.Replace(
                function.Name, "[^A-Za-z0-9 -]", string.Empty);
            return $"{subPayloadPath}/{subPayloadStorageFileName}.raw";
        }
    }
}