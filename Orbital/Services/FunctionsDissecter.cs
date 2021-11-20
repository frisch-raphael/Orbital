using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AsmResolver.PE.File;
using Microsoft.Extensions.Logging;
using Orbital.Factories;
using Orbital.Pocos;
using Orbital.Services.Antivirus;
using Orbital.Static;
using Shared.Dtos;
using Shared.Enums;
using Shared.Pocos;

namespace Orbital.Services
{
    public interface IFunctionsDissecter
    {
        Task<FunctionsDissection> Dissect(List<int> idsOfFunctionsInScope);
    }

    public class FunctionsDissecter
    {
        private readonly ILogger<FunctionsDissecter> Logger;
        private readonly IPeDivider PeDivider;
        private readonly IAntivirusClient AntivirusClient;
        private int NumberOfDockers { get; set; }
        private BackendPayload Payload { get; set; }
        private int NumberToDividePayloadBy => Payload.Functions.Count > 6 ? 6 : Payload.Functions.Count;

        public FunctionsDissecter(
            ILogger<FunctionsDissecter> logger,
            BackendPayload payload,
            IPayloadDividerFactory payloadDividerFactory,
            IAntivirusClient antivirusClient,
            int numberOfDockers)
        {
            Logger = logger;
            Payload = payload;
            AntivirusClient = antivirusClient;
            NumberOfDockers = numberOfDockers;
            PeDivider = payloadDividerFactory.Create(payload);
            ClearRDataSection();
        }



        public async Task<FunctionsDissection> Dissect(List<int> functionsToDissectIds)
        {
            var functionsDissection = new FunctionsDissection
            {
                Antivirus = AntivirusClient.Antivirus,
                DissectionState = OperationState.Ongoing,
                PayloadId = Payload.Id,
                ScanDate = DateTime.Now,
                SubPayloadScanResultRoots = await ScanSubPayloads(functionsToDissectIds)
            };
            return functionsDissection;
        }

        private async Task<List<SubPayloadScanResult>> ScanSubPayloads(ICollection<int> functionsToDissectIds)
        {

            var functionsToDissect = Payload.Functions.Where(f => functionsToDissectIds.Contains(f.Id)).ToList();

            var divideResults =
                await PeDivider.Divide(Payload.StoragePath, functionsToDissect, NumberToDividePayloadBy);
            var subPayloadPathes = divideResults.Select(d => d.SubPayloadFullPath);
            var rawScanResults = await AntivirusClient.ScanAsync(subPayloadPathes.ToArray(), NumberOfDockers);

            var subPayloadScanResults = new List<SubPayloadScanResult>();

            foreach (var divideResult in divideResults)
            {
                var correspondingScanResult = GetCorrespondingScanResult(divideResult, rawScanResults);
                var subPayloadScanResult = new SubPayloadScanResult()
                {
                    FlaggedState = correspondingScanResult.FlaggedState,
                    ScanState = OperationState.Done,
                    SubPayload = new SubPayload()
                    {
                        Functions = functionsToDissect.Where(f => divideResult.FunctionIds.Contains(f.Id)).ToList(),
                        StorageFullPath = divideResult.SubPayloadFullPath
                    }
                };
                if (correspondingScanResult.FlaggedState == FlaggedState.Positive)
                {
                    subPayloadScanResult.SubPayloadScanResultChildren = await ScanSubPayloads(divideResult.FunctionIds);
                }

                subPayloadScanResults.Add(subPayloadScanResult);
            }

            return subPayloadScanResults;
        }

        private static ScanResult GetCorrespondingScanResult(DivideResult divideResult, List<ScanResult> scanResults)
        {
            return scanResults.First(s => s.FilePath == divideResult.SubPayloadFullPath);
        }

        private static void ClearRDataSection()
        {
            throw new NotImplementedException();
        }

    }
}