using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orbital.Attributes;
using Orbital.Factories;
using Orbital.Model;
using Shared.Api.ApiErrors;
using Shared.Dtos;

namespace Orbital.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PayloadsController : ControllerBase
    {
        private readonly ILogger<PayloadsController> Logger;
        private readonly OrbitalContext RodinContext;
        private readonly IPayloadFileStorerFactory PayloadFileStorerFactory;


        public PayloadsController(ILogger<PayloadsController> logger, IPayloadFileStorerFactory payloadStorerFactory, OrbitalContext rodinContext)
        {
            Logger = logger;
            PayloadFileStorerFactory = payloadStorerFactory;
            RodinContext = rodinContext;
        }


        // GET: api/<ValuesController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Payload>> Get()
        {
            return Ok(RodinContext.Payloads.ToList());
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public ActionResult<Payload> Get([Required] int id)
        {
            return Ok(RodinContext.Payloads.Single(p => p.Id == id));
        }

        // POST api/<ValuesController>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Payload>> Post(
            //[Required, MaxFileSize, RestrictFileExtensions] IFormFile file)
            [Required, MaxFileSize] IFormFile file)
        {
            var filePoco = new UploadedFile(file);
            var payloadFileStorer = PayloadFileStorerFactory.Create(filePoco);
            Payload createdPayload;

            try
            {
                await payloadFileStorer.StoreFile();
                createdPayload = payloadFileStorer.StorePayloadDataInDb();
            }
            catch (Exception ex)
            //catch (IOException ex)
            {
                Logger.LogError("Error while trying to save {FileName}. {Message}",
                    filePoco.TrustedFileName, ex.Message);
                if (System.IO.File.Exists(filePoco.StorageFullPath))
                {
                    // cleaning the file
                    Logger.LogInformation("Cleaning {FileName}",
                        filePoco.TrustedFileName);
                    payloadFileStorer.RollStorageBack();
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerError("Error while trying to upload file"));
            }

            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/Payloads/{createdPayload.Id}");
            return new CreatedResult(resourcePath, createdPayload);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Delete([Required] int id)
        {
            var payloadToDelete = RodinContext.Payloads.FirstOrDefault(p => p.Id == id);
            if (payloadToDelete == null) return NotFound(new NotFoundError($"Payload with id {id} does not exist"));
            RodinContext.Payloads.Remove(payloadToDelete);
            RodinContext.SaveChanges();
            return Ok();

        }
    }
}
