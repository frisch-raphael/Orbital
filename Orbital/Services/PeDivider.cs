using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsmResolver.PE.File;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orbital.Pocos;
using Orbital.Static;
using Shared.Dtos;

namespace Orbital.Services
{
    public interface IPeDivider
    {
        Task<List<DivideResult>> Divide(string pathToPe, List<Function> functionsInScope, int dividePayloadBy);
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


        public async Task<List<DivideResult>> Divide(string pathToPe, List<Function> functionsInScope, int dividePayloadBy)
        {
            Logger.LogInformation("Starting to Divide {PePath}", pathToPe);

            var dividedFunctionsInScope = DivideNumberOfFunctions(functionsInScope, dividePayloadBy);

            await Parallel.ForEachAsync(
                dividedFunctionsInScope,
                new ParallelOptions { MaxDegreeOfParallelism = 30 },
                (functions, _) => ExtractAndStoreSubPayload(
                    functions,
                    pathToPe,
                    new FileStream(pathToPe, FileMode.Open, FileAccess.Read)));
            if (DivideResults.Count == 0)
                throw new InvalidOperationException("No subPayload path returned by payload divider");
            return DivideResults;
        }

        private List<Function[]> DivideNumberOfFunctions(List<Function> functionsInScope, int dividePayloadBy)
        {
            var numberOfFunctionsPerChunk = (int)Math.Ceiling((double)functionsInScope.Count / dividePayloadBy);
            return functionsInScope.Chunk(numberOfFunctionsPerChunk).ToList();
        }

        // side effect : add payload path of subpayloads to SubPayloadPathes
        private async ValueTask ExtractAndStoreSubPayload(Function[] functions, string pathToPe, Stream payloadStream)
        {
            Logger.LogInformation(
                "Extracting functions {FunctionIds} from payload {PayloadId}",
                JoinIds(functions), 
                functions[0].BackendPayloadId);

            var subPayloadFullPath = GetStorageFullPath(functions, pathToPe);
            Directory.CreateDirectory(Path.GetDirectoryName(subPayloadFullPath) ??
                                      throw new InvalidOperationException());
            var functionBytes = await ExtractBytesFromFile(function.Length, function.Offset, payloadStream);

            await using var subPayloadStream = new FileStream(subPayloadFullPath, FileMode.Create, FileAccess.Write);
            await subPayloadStream.WriteAsync(functionBytes);

            DivideResults.Add(
                new DivideResult
                {
                    Functions = functions.ToList(),
                    SubPayloadFullPath = subPayloadFullPath
                });
            // var bytes = Encoding.UTF8.GetBytes(content);
            // await subPayloadStream.WriteAsync(bytes, 0, bytes.Length);
        }

        private static string JoinIds(Function[] functions)
        {
            return string.Join('_', functions.Select(f => f.Id));
        }

        private static async Task<byte[]> ExtractBytesFromFile(long length, long offset, Stream file)
        {
            var functionBuffer = new byte[length];
            file.Position = offset;
            await file.ReadAsync(functionBuffer);

            return functionBuffer;
        }

        private static string GetStorageFullPath(Function[] functions, string pathToPe)
        {
            var subPayloadPath = Path.ChangeExtension(pathToPe, null);
            var subPayloadStorageFileName = JoinIds(functions);
            return $"{subPayloadPath}/{subPayloadStorageFileName}.bin";
        }
    }
}