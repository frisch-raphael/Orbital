using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using Orbital.Services;
using Orbital.Factories;
using Shared.Enums;
using Microsoft.Extensions.Logging;
using Orbital.Model;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Rodin.Static;
using Shared.ControllerResponses.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Orbital.Classes;

namespace Orbital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DissectionsController : ControllerBase
    {

        private AntivirusClientFactory AntivirusClientFactory;
        private readonly OrbitalContext OrbitalContext;
        private readonly IServiceScopeFactory ServiceScopeFactory;
        private readonly IHubContext<NotificationHub> HubContext;

        private ILogger<DissectionsController> Logger { get; }

        public DissectionsController(
            AntivirusClientFactory antiviruClientFactory,
            ILogger<DissectionsController> logger,
            OrbitalContext orbitalContext, IServiceScopeFactory serviceScopeFactory, IHubContext<NotificationHub> hubContext)
        {
            AntivirusClientFactory = antiviruClientFactory;
            Logger = logger;
            OrbitalContext = orbitalContext;
            ServiceScopeFactory = serviceScopeFactory;
            HubContext = hubContext;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<List<ScanResult>> Post(
            [Required] DissectionPost dissectionPost)
        {
            var payload = OrbitalContext.Payloads.Single(p => p.Id == dissectionPost.PayloadId);
            var initialResults = new List<ScanResult>();
            return Ok();
            //var scanTasks = new List<Task<List<ScanResult>>>();

            //foreach (var antivirus in scanPost.Antiviruses)
            //{

            //    var antivirusClient = AntivirusClientFactory.Create(antivirus);
            //    scanTasks.Add(antivirusClient.Scan(new string[] { payload.StoragePath }));
            //}

            //await Task.WhenAll(scanTasks);
            //await scanPost.Antiviruses.ForEachAsync(20, async antivirus =>
            //{
            //    var antivirusClient = AntivirusClientFactory.Create(antivirus);
            //    result = await antivirusClient.Scan(new string[] { payload.StorageName });
            //});

            //    Parallel.ForEach(scanPost.Antiviruses, async supportedAntivirus =>
            //    {

            //        using var scope = ServiceScopeFactory.CreateScope();
            //        var orbitalContext = scope.ServiceProvider.GetRequiredService<OrbitalContext>();

            //        var antivirusClient = AntivirusClientFactory.Create(supportedAntivirus);
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
            //            // This controler only takes one payload so only one result
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
