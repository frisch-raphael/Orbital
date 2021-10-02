using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Orbital.Tests.Services
{
    class ControllerContextService
    {
        public static ControllerContext GetFakeControllerContext()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/fakepath";
            context.Request.Host = new HostString("www.fakehost.fr");
            context.Request.Scheme = "https";
            var controllerContext = new ControllerContext()
            {
                HttpContext = context,
            };
            return controllerContext;
        }
    }
}
