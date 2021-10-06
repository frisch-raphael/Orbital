using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shared.Config;

namespace Orbital
{
    public class Program
    {
        public static void Main(string[] args)
        {
            VerifyDockerDemonRunning();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var SharedConfigRoot = JsonConvert.DeserializeObject<SharedOptionsRoot>(
                File.ReadAllText("config.json"));

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(SharedConfigRoot.SharedOptions.orbitalOptions.BaseAddress.AbsoluteUri);
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static void VerifyDockerDemonRunning()
        {
            var pipe = Directory.GetFiles("\\\\.\\pipe\\", "docker_engine");
            if (!(pipe.Length > 0))
            {
                throw new Exception("Docker daemon does not seem to be running.");
            }
        }
    }
}
