using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Api.ApiErrors;

namespace Orbital.Controllers
{
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {

        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [Route("/error")]
        public IActionResult ServerError()
        {
            var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            InternalServerError error;
            if (feature?.Error == null)
            {
                error = new InternalServerError("No exception to handle");
            }
            else
            {

                var st = new StackTrace(feature?.Error, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                var file = Path.GetFileName(frame.GetFileName());
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                var context = string.IsNullOrEmpty(file) ? "Orbital" : $"{file}, {line}";
                error = new InternalServerError($"({context}) : {feature?.Error.Message}");
            }

            return Content(JsonConvert.SerializeObject(error), "application/json");

        }


        //[HttpGet]
        //[Route("{statusCode}")]
        //public IActionResult StatusCodeError(int statusCode)
        //{

        //    var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();

        //    var content = new ExceptionMessageContent() { Error = "Server Error", Message = $"The Server responded with status code {statusCode}" };
        //    return Content(JsonConvert.SerializeObject(content), "application/json");

        //}
    }
}
