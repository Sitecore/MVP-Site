using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using static Mvp.Feature.Forms.Constants;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IConfiguration configuration, ILogger<ApplicationController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

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

        [HttpPost]
        public IActionResult Welcome()
        {
            try
            {
                // TODO :: Backend validation - [make sure Terms checkbox checked].

                // Login into Sitecore to authenticate next operation --> create Item 
                var authData = new Authentication()
                {
                    Domain = _configuration.GetValue<string>("Sitecore:Domain"),
                    Username = _configuration.GetValue<string>("Sitecore:UserName"),
                    Password = _configuration.GetValue<string>("Sitecore:Password")
                };

                var sitecoreUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");

                var AuthUrl = $"{sitecoreUri}{SSCAPIs.AuthenticationApi}";

                var authRequest = (HttpWebRequest)WebRequest.Create(AuthUrl);
                authRequest.Method = "POST";
                authRequest.ContentType = "application/json";

                var requestAuthBody = JsonConvert.SerializeObject(authData);
                var authDatas = new UTF8Encoding().GetBytes(requestAuthBody);

                using (var dataStream = authRequest.GetRequestStream())
                {
                    dataStream.Write(authDatas, 0, authDatas.Length);
                }

                CookieContainer cookies = new CookieContainer();
                authRequest.CookieContainer = cookies;

                var authResponse = authRequest.GetResponse();

                _logger.LogDebug($"Login Status:\n\r{((HttpWebResponse)authResponse).StatusDescription}");

                authResponse.Close();

                // Use SSC to create application Item

                var user = HttpContext.User;
                var identity = (ClaimsIdentity)user?.Identity;
                var createApplication = new CreateApplication
                {
                    ItemName = DateTime.Now.Year.ToString(),
                    TemplateID = _configuration.GetValue<string>("Sitecore:MVPApplicationTemplateId"),
                    FirstName = identity?.FindFirst(_configuration.GetValue<string>("Claims:FirstName"))?.Value,
                    LastName = identity?.FindFirst(_configuration.GetValue<string>("Claims:LastName"))?.Value
                };

                //TODO - how can we get the path to the user item to pass into the item service API
                var createItemUrl = $"{sitecoreUri}{SSCAPIs.CreateItemApi}{"sitecore%2Fcontent%2FMvpSite%2FMVP%20Repository%2FPeople%2Fa%2FAlex%20Lentz?database=master"}"; // Path need to be added to th end ex: https://%3Cdomain%3E/sitecore/api/ssc/item/sitecore%2Fcontent%2Fhome
                var request = (HttpWebRequest)WebRequest.Create(createItemUrl);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.CookieContainer = cookies;

                var requestBody = JsonConvert.SerializeObject(createApplication);

                var data = new UTF8Encoding().GetBytes(requestBody);

                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(data, 0, data.Length);
                }

                var response = request.GetResponse();

                _logger.LogDebug($"Item Status:\n\r{((HttpWebResponse)response).StatusDescription}");

                //Item was created - store item ID in sesion and respond
                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.Created)
                {
                    var createdItemId = response.Headers["Location"].Substring(response.Headers["Location"].LastIndexOf("/"), response.Headers["Location"].Length - response.Headers["Location"].LastIndexOf("/")).Split("?")[0].TrimStart('/');
                    HttpContext.Session.SetString("UserApplicationId", createdItemId);
                    return Json(new { success = true, responseText = "Application succesffuly created." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }
            return Json(new { success = false, responseText = "An error occured while creating your application.  Please contact Sitecore Support." });

        }
    }
}
