using Microsoft.AspNetCore.Mvc;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using Rodin.Services;
using Rodin.Factories;
using Shared.Enums;
using Microsoft.Extensions.Logging;

namespace Rodin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {

        private IAntivirusDockerClientFactory AntivirusDockerClientFactory;
        private ILogger<ScanController> Logger { get; }

        public ScanController(
            IAntivirusDockerClientFactory antivirusDockerClientFactory,
            ILogger<ScanController> logger)
        {
            AntivirusDockerClientFactory = antivirusDockerClientFactory;
            Logger = logger;
        }


        [HttpPost]
        public ActionResult<Dictionary<SupportedAntivirus, string>> Post(List<SupportedAntivirus> antiviruses)
        {
            var response = new Dictionary<SupportedAntivirus, string>();

            Parallel.ForEach(antiviruses, async antivirus =>
            {
                var currentAntivirusDockerClient = AntivirusDockerClientFactory.Create(antivirus);
                if (!await currentAntivirusDockerClient.ExistsImage())
                {
                    await currentAntivirusDockerClient.BuildImage();
                }
            });

            return Ok();
        }


    }
}
