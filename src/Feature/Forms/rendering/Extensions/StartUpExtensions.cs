using Microsoft.AspNetCore.Builder;

namespace Mvp.Feature.Forms.Extensions
{
    public static class StartUpExtensions
    {

        public static void UseFeatureForms(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                      name: "getApplicationlists",
                      pattern: "application/getapplicationlists",

                      new { controller = "Application", action = "GetApplicationLists" }
                 );
                endpoints.MapControllerRoute(
                      name:"getApplicationInfo",
                      pattern: "application/getapplicationinfo",
                  
                      new { controller = "Application", action = "GetApplicationInfo" }
                 );

                endpoints.MapControllerRoute(
                    "submitApplication",
                    "application/submitApplication",
                    new { controller = "Application", action = "SubmitStep" }
                );
            });
        }
    }
}