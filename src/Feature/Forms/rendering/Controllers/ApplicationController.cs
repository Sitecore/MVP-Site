using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    }
}
