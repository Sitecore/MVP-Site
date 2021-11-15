using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Mvp.Feature.BasicContent.Extensions;
using Mvp.Feature.Hero.Extensions;
using Mvp.Feature.Navigation.Extensions;
using Mvp.Feature.Social.Extensions;
using Mvp.Foundation.User.Extensions;
using Mvp.Project.MvpSite.Configuration;
using Mvp.Feature.Forms.Extensions;
using Sitecore.AspNet.ExperienceEditor;
using Mvp.Foundation.People.Extensions;
using Sitecore.AspNet.RenderingEngine.Extensions;
using Sitecore.AspNet.RenderingEngine.Localization;
using Sitecore.LayoutService.Client.Extensions;
using Sitecore.LayoutService.Client.Newtonsoft.Extensions;
using Sitecore.LayoutService.Client.Request;
using Mvp.Foundation.People.Infrastructure;
using Microsoft.AspNetCore.Rewrite;

namespace Mvp.Project.MvpSite.Rendering
{
    public class Startup
    {
        private static readonly string _defaultLanguage = "en";

        public Startup(IConfiguration configuration)
        {
            // Example of using ASP.NET Core configuration binding for various Sitecore Rendering Engine settings.
            // Values can originate in appsettings.json, from environment variables, and more.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1
            Configuration = configuration.GetSection(SitecoreOptions.Key).Get<SitecoreOptions>();

            DotNetConfiguration = configuration;
        }

        public IConfiguration DotNetConfiguration { get; }

        private SitecoreOptions Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting()
                // You must enable ASP.NET Core localization to utilize localized Sitecore content.
                .AddLocalization()
                .AddMvc()
                // At this time the Layout Service Client requires Json.NET due to limitations in System.Text.Json.
                .AddNewtonsoftJson(o => o.SerializerSettings.SetDefaults());

            // Register the Sitecore Layout Service Client, which will be invoked by the Sitecore Rendering Engine.
            services.AddSitecoreLayoutService()
                // Set default parameters for the Layout Service Client from our bound configuration object.
                .WithDefaultRequestOptions(request =>
                {
                    request
                        .SiteName(Configuration.DefaultSiteName)
                        .ApiKey(Configuration.ApiKey);
                })
                .AddHttpHandler("default", Configuration.LayoutServiceUri)
                .AsDefaultHandler();

            // Configure Okta Integration
            services.AddFoundationUser(this.DotNetConfiguration);

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            //    // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
            //    options.HandleSameSiteCookieCompatibility();
            //});

            // Register the Sitecore Rendering Engine services.
            services.AddSitecoreRenderingEngine(options =>
            {
                //Register your components here
                options
                    .AddFoundationUser()
                    .AddFeatureBasicContent()
                    .AddFeatureNavigation()
                    .AddFoundationPeople()
                    .AddFeatureHero()
                    .AddFeatureSocial()
                    .AddFeatureForms()
                    .AddDefaultPartialView("_ComponentNotFound");
            })
                // Includes forwarding of Scheme as X-Forwarded-Proto to the Layout Service, so that
                // Sitecore Media and other links have the correct scheme.
                .ForwardHeaders()
                // Enable forwarding of relevant headers and client IP for Sitecore Tracking and Personalization.
                //.WithTracking()
                // Enable support for the Experience Editor.
                .WithExperienceEditor(options =>
                {
                    // Experience Editor integration needs to know the external URL of your rendering host,
                    // if behind HTTPS termination or another proxy (like Traefik).
                    if (Configuration.RenderingHostUri != null)
                    {
                        options.ApplicationUrl = Configuration.RenderingHostUri;
                    }
                });
            services.AddFoundationPeople();
            // Enable support for robot detection.
            //services.AddSitecoreVisitorIdentification(options =>
            //{
            // Usually the SitecoreInstanceUri is same host as the Layout Service, but it can be any Sitecore CD/CM
            // instance which shares same AspNet session with Layout Service. This address should be accessible
            // from the Rendering Host and will be used to proxy robot detection scripts.
            //    options.SitecoreInstanceUri = Configuration.InstanceUri;
            //});

            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // When running behind HTTPS termination, set the request scheme according to forwarded protocol headers.
            // Also set the Request IP, so that it can be passed on to the Sitecore Layout Service for tracking and personalization.
            app.UseForwardedHeaders(ConfigureForwarding(env));
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                //Add HTTPS redirection, but ignore for healthz path to allow for liveness probes over http
                app.UseWhen(context => !context.Request.Path.StartsWithSegments("/healthz"),
                    builder => builder.UseHttpsRedirection());
            }

            //Add recirects for old mvp pages
            var options = new RewriteOptions()
                            .AddRedirect("mvps/(.*)", "Search?fc_personyear=$1")
                            .AddRedirect("mvps$", "Search");
            app.UseRewriter(options);
            // The Experience Editor endpoint should not be enabled in production DMZ.
            // See the SDK documentation for details.
            if (Configuration.EnableExperienceEditor)
            {
                // Enable the Sitecore Experience Editor POST endpoint.
                app.UseSitecoreExperienceEditor();
            }

            // Standard ASP.NET Core routing and static file support.
            app.UseRouting();
            app.UseStaticFiles();

            // Enable ASP.NET Core Localization, which is required for Sitecore content localization.
            app.UseRequestLocalization(options =>
            {
                // If you add languages in Sitecore which this site / Rendering Host should support, add them here.
                var supportedCultures = new List<CultureInfo> { new CultureInfo(_defaultLanguage) };
                options.DefaultRequestCulture = new RequestCulture(_defaultLanguage, _defaultLanguage);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Allow culture to be resolved via standard Sitecore URL prefix and query string (sc_lang).
                options.UseSitecoreRequestLocalization();
            });

            // Configure Okta Integration
            app.UseFoundationUser();

            app.UseFeatureForms();

            // app.UseCookiePolicy();

            // Enable proxying of Sitecore robot detection scripts
            //app.UseSitecoreVisitorIdentification();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "error",
                    "error",
                    new { controller = "Default", action = "Error" }
                );

                endpoints.MapControllerRoute(
                    "healthz",
                    "healthz",
                    new { controller = "Default", action = "Healthz" }
                );

                endpoints.MapControllerRoute(
                    "getUserEmailClaim",
                    "getUserEmailClaim",
                    new { controller = "Application", action = "GetUserEmailClaim" }
                );

                endpoints.MapControllerRoute(
                    "submitStep1",
                    "submitStep1",
                    new { controller = "Application", action = "Welcome" }
                );

                endpoints.MapControllerRoute(
                    "submitStep2",
                    "submitStep2",
                    new { controller = "Application", action = "Category" }
                );

                endpoints.MapControllerRoute(
                    "submitStep3",
                    "submitStep3",
                    new { controller = "Application", action = "PersonalInformation" }
                );

                endpoints.MapControllerRoute(
                   "submitStep4",
                   "submitStep4",
                   new { controller = "Application", action = "ObjectivesandMotivation" }
               );

                endpoints.MapControllerRoute(
                   "submitStep5",
                   "submitStep5",
                   new { controller = "Application", action = "Socials" }
               );

                endpoints.MapControllerRoute(
                  "submitStep6",
                  "submitStep6",
                  new { controller = "Application", action = "NotableCurrentYearContributions" }
              );

                endpoints.MapControllerRoute(
                 "submitStep7",
                 "submitStep7",
                 new { controller = "Application", action = "Confirmation" }
             );

                endpoints.MapControllerRoute(
                 "getCategories",
                 "getCategories",
                 new { controller = "Application", action = "GetCategories" }
             );



                // Enables the default Sitecore URL pattern with a language prefix.
                endpoints.MapSitecoreLocalizedRoute("sitecore", "Index", "Default");

                // Fall back to language-less routing as well, and use the default culture (en).
                endpoints.MapFallbackToController("Index", "Default");

            });

        }

        private ForwardedHeadersOptions ConfigureForwarding(IWebHostEnvironment env)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
                
            // Allow forwarding of headers from Traefik in development & NGINX in k8s
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            return options;
        }
    }
}
