using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mvp.Feature.Forms.Models;
using Mvp.Foundation.User.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : Controller
    {

        [HttpGet]
        public IActionResult GetUserEmailClaim()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(string.Empty);

            //First get user claims    
            var claims = User.Claims.ToList();
            //Filter specific claim    
            var claim = claims?.FirstOrDefault(x => x.Type.Equals("email", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(claim))
                return Json(string.Empty);

            return Json(claim);
        }

        private static void AddOktaAuthHeaders(WebRequest request, HttpContext httpContext)
        {
                var str = GetAuthenticationHeader(httpContext);

                if (string.IsNullOrEmpty(str))
                    return;

                if (request.Headers.AllKeys.Contains("authorization"))
                    return;

                 request.Headers.Add("authorization","Bearer " + str);
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

        [HttpGet]
        public IActionResult GetApplicationInfo()
        {
            //TODO:need to pass identifier from the currently logged in user

            //if (Sitecore.Context.IsLoggedIn && Sitecore.Context.User.Identity.IsAuthenticated) 
            if (!User.Identity.IsAuthenticated)
                return null;
 
            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create("http://cd/api/sitecore/Application/GetApplicationInfo");

            AddOktaAuthHeaders(request, HttpContext);
            
            // Set the Method property of the request to POST.
            request.Method = "GET";
 
            // Get the response.
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            var responseFromServer = string.Empty;
            using (var dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
            }

            // Close the response.
            response.Close();
            return Json(responseFromServer);

        } 
        [HttpPost]
        public IActionResult SubmitStep(string input)
        {
            var appModel = new ApplicationModel();
            return Json(appModel);
        }
    }
}
