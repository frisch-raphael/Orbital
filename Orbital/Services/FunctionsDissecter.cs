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
        private int NumberOfDockers { get; set;}
        private BackendPayload Payload { get; set;}
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
            var functionsDissection = new FunctionsDissection()
            {
                Antivirus = AntivirusClient.Antivirus,
                DissectionState = OperationState.Ongoing,
                PayloadId = Payload.Id,
                ScanDate = DateTime.Now
            };
            functionsDissection.SubPayloadScanResultRoots = await ScanSubPayloads(functionsToDissectIds);
            return functionsDissection;
        }

        private async Task<List<SubPayloadScanResult>> ScanSubPayloads( 
            List<int> functionsToDissectIds, 
            int recursionLevel = 0)
        {
            if (recursionLevel == 0)
            {

            }
            var functionsToDissect = Payload.Functions.Where(f => functionsToDissectIds.Contains(f.Id)).ToList();

            var divideResults = await PeDivider.Divide(Payload.StoragePath, functionsToDissect, NumberToDividePayloadBy);
            var subPayloadPathes = divideResults.Select(d => d.SubPayloadFullPath);
            var rawScanResults = await AntivirusClient.ScanAsync(subPayloadPathes.ToArray(), NumberOfDockers);
            foreach (var divideResult in divideResults)
            {
                if (GetCorrespondingScanResult(divideResult, rawScanResults).FlaggedState == FlaggedState.Negative) break;
                await ScanSubPayloads(divideResult.FunctionIds, recursionLevel + 1);
            }
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