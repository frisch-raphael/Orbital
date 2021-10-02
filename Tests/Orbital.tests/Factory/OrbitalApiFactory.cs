using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orbital.Model;
using Orbital.Shared.Static;

namespace Orbital.Shared
{
    public class OrbitalApiFactory : WebApplicationFactory<Orbital.Startup>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrbitalContext>));

                services.Remove(descriptor);

                services.AddDbContext<OrbitalContext>((options, context) =>
                    {
                        context.UseSqlite("Data Source=orbitaltest.db");
                    }
                );

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<OrbitalContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<OrbitalApiFactory>>();

                    try
                    {
                        DatabaseSeeding.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }

    }
}
