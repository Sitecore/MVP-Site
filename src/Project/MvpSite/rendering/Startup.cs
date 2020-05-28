using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvp.Feature.Navigation.Extensions;
using Sitecore.AspNet.RenderingEngine.Extensions;
using Sitecore.LayoutService.Client.Extensions;
using Sitecore.LayoutService.Client.Newtonsoft.Extensions;
using Sitecore.LayoutService.Client.Request;

namespace Mvp.Project.MvpSite.Rendering
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
            
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            const string PROTOCOL_HEADER = "X-FORWARDED-PROTO";

            services
                .AddRouting()
                .AddMvc()
                .AddNewtonsoftJson(o => o.SerializerSettings.SetDefaults());

            // Register the Sitecore layout service client
            services.AddSitecoreLayoutService()
                .WithDefaultRequestOptions(request =>
                {
                    request
                        .Language(configuration["Sitecore:DefaultLanguage"])
                        .SiteName(configuration["Sitecore:DefaultSiteName"])
                        .ApiKey(configuration["Sitecore:ApiKey"]);
                })
                .AddHttpHandler("default", new Uri(configuration["Sitecore:LayoutService"]))
                //SITECORE TODO: OOTB support for passing this header for media URLs behind SSL termination
                .MapFromRequest((layoutRequest, httpRequest) =>
                {
                    if (layoutRequest.TryGetValue(PROTOCOL_HEADER, out var proto))
                    {
                        httpRequest.Headers.Add(PROTOCOL_HEADER, proto.ToString());
                    }
                })
                .AsDefaultHandler();

            // Register the Sitecore rendering engine.
            services.AddSitecoreRenderingEngine(options =>
            {
                options
                    //SITECORE TODO: OOTB support for passing this header for media URLs behind SSL termination
                    .MapToRequest((httpRequest, layoutRequest) =>
                    {
                        layoutRequest.Add(PROTOCOL_HEADER, httpRequest.Headers[PROTOCOL_HEADER].ToString());
                    })

                    // TODO: register your components here
                   .AddFeatureNavigation()
                   .AddDefaultPartialView("_ComponentNotFound");
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                // any routes that do not have otherwise-specified routing will fall into being rendered via Sitecore
                endpoints.MapFallbackToController("index", "default");
                //SITECORE TODO: Is there a way to avoid explicit mapping for error route?
                endpoints.MapControllerRoute(
                    name: "error",
                    pattern: "error",
                    defaults: new { controller = "Default", action = "Error" }
                );
            });
        }
    }
}
