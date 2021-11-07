using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbital.Classes;
using Orbital.Factories;
using Orbital.Model;
using Orbital.Pocos;
using Orbital.Services.Antivirus;
using Shared.ControllerResponses.Dtos;
using Shared.Dtos;
using Shared.Enums;
using Shared.Pocos;

namespace Orbital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DissectionsController : ControllerBase
    {
        private readonly AntivirusClientFactory AntivirusesClientFactory;
        private readonly OrbitalContext OrbitalContext;
        private readonly IPayloadDividerFactory PayloadDividerFactory;
        private readonly IServiceScopeFactory ServiceScopeFactory;
        private readonly IHubContext<NotificationHub> HubContext;

        private ILogger<DissectionsController> Logger { get; }

        public DissectionsController(
            AntivirusClientFactory antivirusClientFactory,
            ILogger<DissectionsController> logger,
            OrbitalContext orbitalContext,
            IPayloadDividerFactory payloadDividerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IHubContext<NotificationHub> hubContext)
        {
            AntivirusesClientFactory = antivirusClientFactory;
            Logger = logger;
            OrbitalContext = orbitalContext;
            PayloadDividerFactory = payloadDividerFactory;
            ServiceScopeFactory = serviceScopeFactory;
            HubContext = hubContext;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<List<Scan>>> Post(
            [Required] DissectionPost dissectionPost)
        {
            var payload = OrbitalContext.BackendPayloads.Single(p => p.Id == dissectionPost.PayloadId);

            await HubContext.Clients.All.SendAsync(
                Notifications.DissectionStarted.ToString(),
                new DissectionResultWsMessage { Payload = payload });

            await OrbitalContext.Entry(payload)
                .Collection(p => p.Functions)
                .LoadAsync();


                
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            var initialResult = StoreInitDissectResultInDb(dissectionPost);
            Dissect(dissectionPost, payload, initialResult).SafeFireAndForget();
            return new CreatedResult(resourcePath, initialResult);

        }

        private async Task Dissect(DissectionPost dissectionPost, BackendPayload payload, EntityEntry<Dissection> initialResult)
        {

            using var scope = ServiceScopeFactory.CreateScope();
            var orbitalContext = scope.ServiceProvider.GetRequiredService<OrbitalContext>();

            try
            {

                var antivirusClient = AntivirusesClientFactory.Create(dissectionPost.SupportedAntivirus);
                // var divideResults = await PayloadDividerFactory.Create(payload).Divide(dissectionPost.FunctionIds);
                var divideResults = await PayloadDividerFactory.Create(payload).DivideInN(2);
                var subPayloadPathes = divideResults.Select(d => d.SubPayloadFullPath);
                var rawScanResults = await antivirusClient.ScanAsync(subPayloadPathes.ToArray(), dissectionPost.NumberOfDocker);
                var subPayloadScanResults = rawScanResults.Select(rawScanResult => new SubPayloadScanResult()
                {

                    FlaggedState = rawScanResult.FlaggedState,
                    ScanState = rawScanResult.IsError ? OperationState.Error : OperationState.Done,
                    SubPayload = new SubPayload()
                    {
                        Functions = divideResults
                            .Single(d => d.SubPayloadFullPath == rawScanResult.FilePath)
                            .FunctionIds
                            .Select(f1Id => orbitalContext.Functions.Single(f2 => f1Id == f2.Id)).ToList(),
                        StorageFullPath = rawScanResult.FilePath

                    }

                });
                initialResult.Entity.SubPayloadScanResult = subPayloadScanResults.ToList();
                orbitalContext.DissectResults.Update(initialResult.Entity);
                initialResult.Entity.DissectionState = OperationState.Done;
                await orbitalContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                Logger.LogError("Error while dissecting payload");
                initialResult.Entity.DissectionState = OperationState.Error;
                await orbitalContext.SaveChangesAsync();
                throw;
            }

        }

        private EntityEntry<Dissection> StoreInitDissectResultInDb(DissectionPost dissectionPost)
        {
            var initialResult = new Dissection() 
                { 
                    Antivirus = dissectionPost.SupportedAntivirus, 
                    PayloadId = dissectionPost.PayloadId, 
                    ScanDate = DateTime.Now,
                    DissectionState = OperationState.Ongoing
                    
                };
            var dissectEntity = OrbitalContext.DissectResults.Add(initialResult);
            OrbitalContext.SaveChangesAsync();
            return dissectEntity;
        }
    }
}