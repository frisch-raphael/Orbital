using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsmResolver.PE.File;
using Microsoft.Extensions.Logging;
using Orbital.Pocos;
using Orbital.Static;
using Shared.Dtos;

namespace Orbital.Services
{
    public interface IPeDivider
    {
        Task<List<DivideResult>> Divide(string pathToPe, List<Function> functionsInPe, int dividePayloadBy);
    }

    public class PeDivider : IPeDivider
    {
        private readonly ILogger<PeDivider> Logger;

        public PeDivider(ILogger<PeDivider> logger)
        {
            Logger = logger;
        }

        private List<DivideResult> DivideResults { get; } = new();


        // this is temporary
        // public async Task<List<DivideResult>> DivideInN(int numberOfParts)
        // {
        //
        //     for (var i = 0; i < numberOfParts; i++)
        //     {
        //         var partPath = $"{LocalPathes.UploadDirectory}part_{i}.raw";
        //         var payloadPartStream = new FileStream(BackendPayload.StoragePath, FileMode.Open, FileAccess.Read);
        //
        //         var offset = i * (payloadPartStream.Length / numberOfParts);
        //
        //         var partBytes = await ExtractBytesFromFile(payloadPartStream.Length / numberOfParts, offset, payloadPartStream);
        //         await using var partPayloadStream =  new FileStream(partPath, FileMode.Create, FileAccess.Write);
        //         await partPayloadStream.WriteAsync(partBytes);
        //
        //         DivideResults.Add(
        //             new DivideResult
        //             {
        //                 FunctionIds = new List<int> { },
        //                 SubPayloadFullPath = partPath
        //             });
        //
        //
        //     }
        //     return DivideResults;
        //
        // }

        // public async Task<List<DivideResult>> DivideSection()
        // {
        //     var peFile = PEFile.FromFile(BackendPayload.StoragePath);;
        //     foreach (var section in peFile.Sections)
        //     {
        //         Console.WriteLine(section.Name);
        //     }
        // }


        public async Task<List<DivideResult>> Divide(string pathToPe, List<Function> functionsInPe, int dividePayloadBy)
        {
            Logger.LogInformation("Starting to Divide {PePath}", pathToPe);



            await Parallel.ForEachAsync(
                functionsInPe,
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

            await using var subPayloadStream = new FileStream(subPayloadFullPath, FileMode.Create, FileAccess.Write);
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