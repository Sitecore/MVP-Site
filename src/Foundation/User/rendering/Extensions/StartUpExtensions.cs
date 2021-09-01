using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Okta.AspNetCore;

namespace Mvp.Foundation.User.Extensions
{
    public static class StartUpExtensions
    {
        public static void AddFoundationUser(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/user/signin");
            })
            .AddOktaMvc(new OktaMvcOptions
            {
                // Settings from Project MvpSite appsettings.json
                OktaDomain = configuration.GetValue<string>("Okta:OktaDomain"),
                ClientId = configuration.GetValue<string>("Okta:ClientId"),
                ClientSecret = configuration.GetValue<string>("Okta:ClientSecret"),
                AuthorizationServerId = configuration.GetValue<string>("Okta:AuthorizationServerId"),
                Scope = new List<string> { "openid", "profile", "email" },
            });
        }


        public static void UseFoundationUser(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "OktaSignin",
                    pattern: "user/{action=SignIn}",
                    defaults: new { controller = "User" });
            });
        }
    }
}