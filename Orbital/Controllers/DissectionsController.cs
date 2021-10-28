using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbital.Classes;
using Orbital.Factories;
using Orbital.Model;
using Shared.ControllerResponses.Dtos;
using Shared.Dtos;
using Shared.Enums;

namespace Orbital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DissectionsController : ControllerBase
    {
        private AntivirusClientFactory AntivirusesClientFactory;
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
        public async Task<ActionResult<List<ScanResult>>> Post(
            [Required] DissectionPost dissectionPost)
        {
            var payload = OrbitalContext.BackendPayloads.Single(p => p.Id == dissectionPost.PayloadId);
            await OrbitalContext.Entry(payload)
                .Collection(p => p.Functions)
                .LoadAsync();

            var initialResults = new List<ScanResult>();
            // var subPayloads = OrbitalContext.SubPayloads.Where(p => p.Payload.Id == p.Id);
            //
            // OrbitalContext.RemoveRange(subPayloads);
            // OrbitalContext.SubPayloads.AddRange(divider.Divide());

            await OrbitalContext.SaveChangesAsync();

            PayloadDividerFactory.Create(payload).Divide(dissectionPost.FunctionIds);
            await HubContext.Clients.All.SendAsync(
                Notifications.DissectionStarted.ToString(),
                new DissectionResultWsMessage { Payload = payload });


            return Ok();
            //var scanTasks = new List<Task<List<ScanResult>>>();

            //foreach (var antivirus in scanPost.Antiviruses)
            //{

            //    var antivirusClient = AntivirusesClientFactory.Create(antivirus);
            //    scanTasks.Add(antivirusClient.Scan(new string[] { payload.StoragePath }));
            //}

            //await Task.WhenAll(scanTasks);
            //await scanPost.Antiviruses.ForEachAsync(20, async antivirus =>
            //{
            //    var antivirusClient = AntivirusesClientFactory.Create(antivirus);
            //    result = await antivirusClient.Scan(new string[] { payload.StorageName });
            //});

            //    Parallel.ForEach(scanPost.Antiviruses, async supportedAntivirus =>
            //    {

            //        using var scope = ServiceScopeFactory.CreateScope();
            //        var orbitalContext = scope.ServiceProvider.GetRequiredService<OrbitalContext>();

            //        var antivirusClient = AntivirusesClientFactory.Create(supportedAntivirus);
            //        var initialResult = new ScanResult()
            //        {
            //            Antivirus = supportedAntivirus,
            //            PayloadId = payload.Id,
            //            ScanDate = DateTime.Now
            //        };

            //        var resultEntity = orbitalContext.ScanResults.Add(initialResult);
            //        initialResult.Id = resultEntity.Entity.Id;
            //        initialResults.Add(initialResult);
            //        await orbitalContext.SaveChangesAsync();


            //        try
            //        {
            //            await HubContext.Clients.All.SendAsync(
            //                Notifications.ScanStarted.ToString(),
            //                new ScanResultWSMessage { Payload = payload, ScanResult = resultEntity.Entity });
            //            var result = await antivirusClient.Scan(new string[] { payload.StoragePath });
            //            // This controller only takes one payload so only one result
            //            resultEntity.Entity.IsFlagged = result[0].IsFlagged;
            //            resultEntity.Entity.isDone = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.LogError("Scanning with {Antivirus} failed : {ErrorMessage}", supportedAntivirus, ex.Message);
            //            resultEntity.Entity.isError = true;
            //        }

            //        await orbitalContext.SaveChangesAsync();
            //        await HubContext.Clients.All.SendAsync(
            //            Notifications.ScanDone.ToString(),
            //            new ScanResultWSMessage { Payload = payload, ScanResult = resultEntity.Entity });

            //    });

            //    var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            //    //var scanControllerResponses = scanTasks.Select(st => new ScanResponse()
            //    //{
            //    //    Antivirus = st.Result[0].Antivirus,
            //    //    IsFlagged = st.Result[0].IsFlagged

            //    //});
            //    return new CreatedResult(resourcePath, initialResults);
        }
    }
}