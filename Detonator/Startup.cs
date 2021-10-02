using System.Linq;
using Rodin.Factories;
using Rodin.Services;
using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Shared.Api.ApiErrors;

namespace Rodin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        return CustomErrorResponse(actionContext);
                    };
                });

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NajiN Detonator API", Version = "v1" });
            });

            services.AddSingleton<DockerClientConfiguration>();
            services.AddSingleton<IAntivirusDockerClientFactory, AntivirusDockerClientFactory>();
            services.AddSingleton<IDefenderDockerClient, DefenderDockerClient>();
            services.AddSingleton<IClamavDockerClient, ClamavDockerClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "NajiN Detonator API v1");
            });
        }
        private BadRequestObjectResult CustomErrorResponse(ActionContext actionContext)
        {
            return new BadRequestObjectResult(actionContext.ModelState
             .Where(modelError => modelError.Value.Errors.Count > 0)
             .Select(modelError => new BadRequestError($"There was an error with the field '{modelError.Key}' : " +
                $"{modelError.Value.Errors.FirstOrDefault().ErrorMessage}"))
             .First());
        }
    }
}
