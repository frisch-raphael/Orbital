using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Orbital.Factories;
using Shared.Enums;
using Microsoft.Extensions.Logging;
using Orbital.Model;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Shared.ControllerResponses.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Orbital.Classes;

namespace Orbital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScansController : ControllerBase
    {

        private readonly AntivirusClientFactory AntivirusesClientFactory;
        private readonly OrbitalContext OrbitalContext;
        private readonly IServiceScopeFactory ServiceScopeFactory;
        private readonly IHubContext<NotificationHub> HubContext;

        private ILogger<ScansController> Logger { get; }

        public ScansController(
            AntivirusClientFactory antivirusClientFactory,
            ILogger<ScansController> logger,
            OrbitalContext orbitalContext, IServiceScopeFactory serviceScopeFactory, IHubContext<NotificationHub> hubContext)
        {
            AntivirusesClientFactory = antivirusClientFactory;
            Logger = logger;
            OrbitalContext = orbitalContext;
            ServiceScopeFactory = serviceScopeFactory;
            HubContext = hubContext;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<List<ScanResult>> Post(
            [Required] ScanPost scanPost)
        {
            var payload = OrbitalContext.Payloads.Single(p => p.Id == scanPost.PayloadId);
            var initialResults = new List<ScanResult>();

            async void ScanBody(SupportedAntivirus supportedAntivirus)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var orbitalContext = scope.ServiceProvider.GetRequiredService<OrbitalContext>();

                var antivirusClient = AntivirusesClientFactory.Create(supportedAntivirus);
                var initialResult = new ScanResult() { Antivirus = supportedAntivirus, PayloadId = payload.Id, ScanDate = DateTime.Now };

                var resultEntity = orbitalContext.ScanResults.Add(initialResult);
                initialResult.Id = resultEntity.Entity.Id;
                initialResults.Add(initialResult);
                await orbitalContext.SaveChangesAsync();


                try
                {
                    await HubContext.Clients.All.SendAsync(
                        Notifications.ScanStarted.ToString(), 
                        new ScanResultWsMessage { Payload = payload, ScanResult = resultEntity.Entity });
                    var result = await antivirusClient.Scan(new[] { payload.StoragePath });
                    // This controller only takes one payload so only one result
                    resultEntity.Entity.IsFlagged = result[0].IsFlagged;
                    resultEntity.Entity.IsDone = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError("Scanning with {Antivirus} failed : {ErrorMessage}", supportedAntivirus, ex.Message);
                    resultEntity.Entity.IsError = true;
                }

                await orbitalContext.SaveChangesAsync();
                await HubContext.Clients.All.SendAsync(Notifications.ScanDone.ToString(), new ScanResultWsMessage { Payload = payload, ScanResult = resultEntity.Entity });
            }

            Parallel.ForEach(scanPost.Antiviruses, ScanBody);

            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            return new CreatedResult(resourcePath, initialResults);
        }


    }
}
