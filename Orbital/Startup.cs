using System.Linq;
using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Orbital.Services.AntivirusBackends;
using Orbital.Factories;
using Orbital.Model;
using Orbital.Services;
using Shared.Api.ApiErrors;
using Microsoft.EntityFrameworkCore;
using Orbital.Services.Antivirus;
using System.Net.WebSockets;
using System.Net;
using Orbital.Classes;

namespace Orbital
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
            services.AddCors(options =>
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                })
            );

            services.AddSignalR();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.UseMemberCasing();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());

                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    //InvalidModelStateResponseFactory is a Func delegate  
                    // and used to customize the error response.    
                    //It is exposed as property of ApiBehaviorOptions class  
                    // that is used to configure api behaviour.    
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        //CustomErrorResponse is method that gets model validation errors     
                        //using ActionContext creates customized response,  
                        // and converts invalid model state dictionary    

                        return CustomErrorResponse(actionContext);
                    };
                });



            services.AddDbContext<OrbitalContext>((options, context) =>
            {
                context.UseSqlite("Data Source=orbital.db");
            }
            );
            //var sp = services.BuildServiceProvider();

            //using (var scope = sp.CreateScope())
            //{
            //    var scopedServices = scope.ServiceProvider;
            //    var db = scopedServices.GetRequiredService<OrbitalContext>();
            //    db.Database.EnsureCreated(); 
            //}

            services.AddSingleton<IFunctionService, FunctionService>();
            services.AddScoped<HammerWrapper>();
            services.AddScoped<IPayloadFileStorerFactory, PayloadFileStorerFactory>();

            services.AddSingleton<DockerClientConfiguration>();
            services.AddSingleton<IDockerClient>(provider =>
            {
                var dependency = provider.GetRequiredService<DockerClientConfiguration>();

                // You can select the constructor you want here.
                return dependency.CreateClient();
            });
            services.AddSingleton<IAntivirusImageBuilder, AntivirusImageBuilder>();
            services.AddSingleton<IAntivirusContainerLauncherFactory, AntivirusContainerLauncherFactory>();
            services.AddSingleton<IScannerServiceFactory, ScannerServiceFactory>();
            services.AddSingleton<IAntivirusBackendFactory, AntivirusBackendFactory>();
            services.AddSingleton<IPayloadDividerFactory, PayloadDividerFactory>();
            services.AddSingleton<IPeFunctionOffsetGetter, PeFunctionOffsetGetter>();
            services.AddSingleton<IPayloadDeleter, PayloadDeleter>();
            services.AddSingleton<AntivirusClientFactory>();

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orbital API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrbitalContext orbitalContext)
        {
            app.UseWebSockets();

            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseExceptionHandler("/error");

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

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
                c.SwaggerEndpoint("v1/swagger.json", "NajiN Rodin API v1");
            });

            orbitalContext.Database.EnsureCreated();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/scan_notifications");
                endpoints.MapControllers();
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
