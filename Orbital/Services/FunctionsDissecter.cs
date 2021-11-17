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
        private readonly BackendPayload Payload;
        private readonly IPayloadDivider PayloadDivider;
        private readonly IAntivirusClientFactory AntivirusClientFactory;
        public FunctionsDissecter(
            ILogger<FunctionsDissecter> logger, 
            BackendPayload payload, 
            IPayloadDividerFactory payloadDividerFactory, 
            IAntivirusClientFactory antivirusClientFactory)
        {
            Logger = logger;
            Payload = payload;
            AntivirusClientFactory = antivirusClientFactory;
            PayloadDivider = payloadDividerFactory.Create(payload);
            ClearRDataSection();
        }


        public async Task<FunctionsDissection> Dissect(
            BackendPayload payload, 
            List<int> idsOfFunctionsInScope, 
            SupportedAntivirus antivirus,
            int numberOfDockers, 
            int recursionLevel = 0)
        {
            if (recursionLevel == 0)
            {

            }
            var antivirusClient = AntivirusClientFactory.Create(antivirus);

            var divideResults = await PayloadDivider.Divide(idsOfFunctionsInScope, numberOfDockers);
            var subPayloadPathes = divideResults.Select(d => d.SubPayloadFullPath);
            var rawScanResults = await antivirusClient.ScanAsync(subPayloadPathes.ToArray(), numberOfDockers);

        }


        private void ClearRDataSection()
        {
            throw new NotImplementedException();
        }
}