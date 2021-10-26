using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet]
        public IActionResult GetApplicationInfo()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            //First get user claims    
            var claims = User.Claims.ToList();
            //Filter specific claim    
            var email = claims?.FirstOrDefault(x => x.Type.Equals("email", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(email))
                return null;

            // Create a request using a URL that can receive a post.
            WebRequest request = WebRequest.Create("https://mvp-cm.sc.localhost/Application/GetUserCurrentStep");
            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            var myData = new
            {
                identifier = email
            };
            string jsonData = JsonConvert.SerializeObject(myData);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            // The using block ensures the stream is automatically closed.
            var responseFromServer = string.Empty;
            using (dataStream = response.GetResponseStream())
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

        [HttpPost("welcome")]
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

                var createApplication = new CreateApplication
                {
                    ItemName = DateTime.Now.Year.ToString("yyyy"),
                    TemplateId = _configuration.GetValue<string>("Sitecore:MVPApplicationTemplateId")
                };

                var createItemUrl = $"{sitecoreUri}{SSCAPIs.CreateItemApi}"; // Path need to be added to th end ex: https://%3Cdomain%3E/sitecore/api/ssc/item/sitecore%2Fcontent%2Fhome
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

                // TODO :: If success --> status equals 201 aka "created" move user to next step
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }


            return PartialView();
        }
    }
}
