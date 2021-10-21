using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;
using Sitecore.LayoutService.Client.Request;

namespace Mvp.Foundation.User.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFoundationUser(this RenderingEngineOptions options)
        {
            options.AddPartialView("SignIn");

            AddOktaAuthHeaders(options);

            return options;
        }

        // =======================================================================

        /// <summary>
        /// Pass Okta id_token as header from rendering host to sitecore app for login
        /// </summary>
        /// <param name="options"></param>
        private static void AddOktaAuthHeaders(RenderingEngineOptions options)
        {
            options.MapToRequest((httpRequest, sitecoreLayoutRequest) =>
            {
                IDictionary<string, string[]> headers =
                    new Dictionary<string, string[]>(System.StringComparer.OrdinalIgnoreCase);

                var str = GetAuthenticationHeader(httpRequest.HttpContext);
                if (string.IsNullOrEmpty(str))
                    return;

                if (headers.ContainsKey("authorization"))
                    return;

                headers.Add("authorization", new[]
                {
                        "Bearer " + str
                    });

                sitecoreLayoutRequest.AddHeaders(headers);
            });
        }

        private static string GetAuthenticationHeader(HttpContext context)
        {
            //This simply gets the same token that the app uses, you can use MSAL to get a new token specifically for this "API" call
            try
            {
                return context.GetTokenAsync("id_token").Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting token: " + e);
                return string.Empty;
            }
        }
    }
}