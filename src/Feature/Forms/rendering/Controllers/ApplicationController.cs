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
using Mvp.Feature.Forms.Shared.Models;

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
        public IActionResult GetApplicationLists()
        {
            //TODO:need to pass identifier from the currently logged in user

            //if (Sitecore.Context.IsLoggedIn && Sitecore.Context.User.Identity.IsAuthenticated) 
            if (!User.Identity.IsAuthenticated)
                return null;

            // Create a request using a URL that can receive a post.
            var sitecoreCdUri = _configuration.GetValue<string>("Sitecore:InstanceUri");
            WebRequest request = WebRequest.Create($"{sitecoreCdUri}/api/sitecore/Application/GetApplicationLists");

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

        [HttpGet]
        public IActionResult GetApplicationInfo()
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { IsLoggedIn = false});

            ApplicationInfo applicationInfo = GetApplication();
            if (applicationInfo!=null)
			{
                if(applicationInfo.Status == ApplicationStatus.NotLoggedIn)
				{
                    return Json(new { IsLoggedIn = false });
                }
                else if (applicationInfo.Status == ApplicationStatus.PersonItemNotFound)
				{
                    return Json(new { IsLoggedIn = true, ApplicationAvailable = false });
                }
                else if (applicationInfo.Status == ApplicationStatus.ApplicationItemNotFound)
                {
                    return Json(new { IsLoggedIn = true, ApplicationAvailable = false });
                }
                else if (applicationInfo.Status == ApplicationStatus.ApplicationFound)
                {
                    return Json(new { IsLoggedIn = true, ApplicationAvailable = true, Result = applicationInfo });
                }
                else if (applicationInfo.Status == ApplicationStatus.ApplicationCompleted)
                {
                    return Json(new { IsLoggedIn = true, ApplicationCompleted = true });
                }

            }

            return Json(new { IsLoggedIn = false, ApplicationAvailable = false});

        }

        private ApplicationInfo GetApplication()
        {
            // Create a request using a URL that can receive a post.
            var sitecoreCdUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");
            WebRequest request = WebRequest.Create($"{sitecoreCdUri}/api/sitecore/Application/GetApplicationInfo");

            var user = HttpContext.User;
            var identity = (ClaimsIdentity)user?.Identity;
            string oktaId = identity?.FindFirst(_configuration.GetValue<string>("Claims:OktaId"))?.Value;
            var email = identity?.FindFirst(_configuration.GetValue<string>("Claims:Email"))?.Value;

            AddOktaAuthHeaders(request, HttpContext);

            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.ContentType = "application/json";

            string requestData = JsonConvert.SerializeObject(new
            {
                identifier = oktaId,
                email = email
            });

            var data = new UTF8Encoding().GetBytes(requestData);

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(data, 0, data.Length);
            }
 
            // Get the response.
            WebResponse response = request.GetResponse();

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
            ApplicationInfo applicationInfo = JsonConvert.DeserializeObject<ApplicationInfo>(responseFromServer);
            return applicationInfo;
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
                var user = HttpContext.User;
                var identity = (ClaimsIdentity)user?.Identity;
                var firstName = identity?.FindFirst(_configuration.GetValue<string>("Claims:FirstName"))?.Value;
                var lastName = identity?.FindFirst(_configuration.GetValue<string>("Claims:LastName"))?.Value;
                var email = identity?.FindFirst(_configuration.GetValue<string>("Claims:Email"))?.Value;
                var OktaId = identity?.FindFirst(_configuration.GetValue<string>("Claims:OktaId"))?.Value;

                string applicationId = "";
                var application = GetApplication();
                if(application!=null && application.Status == ApplicationStatus.PersonItemNotFound)
				{
                   var personPathNId =  CreatePersonItem(firstName, lastName, OktaId, email);

                    applicationId = CreateApplicationItem(firstName, lastName, personPathNId.Split("||")[1], personPathNId.Split("||")[0], true);
				}
                else if (application != null && application.Status == ApplicationStatus.ApplicationItemNotFound)
				{
                    applicationId = CreateApplicationItem(firstName, lastName, application.Person.ItemPath, application.Person.ItemId, true); ;
                }
                else if (application != null && application.Status == ApplicationStatus.ApplicationFound)
                {
                    applicationId = application.Application.ApplicationId;

                }
                if(!string.IsNullOrEmpty(applicationId))
				{
                    dynamic dataToUpdate = new
                    {
                        Application = "{" + applicationId.ToUpper() + "}",
                        Step = ItemsIds.ApplicationSteps.Category
                    };

                    UpdateItemInSc(applicationId, dataToUpdate);
                    return Json(new { success = true, responseText = "Application succesffuly created.", applicationItemId = applicationId });
                }
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
            }
            return Json(new { success = false, responseText = "An error occured while creating your application.  Please contact Sitecore Support." });

        }

        private string CreatePersonItem(string firstName, string lastName, string oktaId, string email)
		{
            // Login into Sitecore to authenticate next operation --> create Item 
            var cookies = Authenticate();

            // Use SSC to create application Item
            var sitecoreUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");

            var createPerson = new CreatePerson
            {
                ItemName = firstName + " " + lastName,
                TemplateID = _configuration.GetValue<string>("Sitecore:PersonTemplateId"),
                FirstName = firstName,
                LastName = lastName,
                OktaId = oktaId,
                Email = email
            };

            var createItemUrl = $"{sitecoreUri}{SSCAPIs.ItemApi}{$"sitecore%2Fcontent%2FMvpSite%2FMVP%20Repository%2FPeople?database=master"}"; 
            var request = (HttpWebRequest)WebRequest.Create(createItemUrl);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.CookieContainer = cookies;

            var requestBody = JsonConvert.SerializeObject(createPerson);

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
                string itemPath = GetItemPath(createdItemId);
                return createdItemId + "||" + itemPath;
            }

            return string.Empty;
		}

        private void UpdateItemInSc(string itemId, object dataToUpdate)
		{
            var sitecoreUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");
            var updateItemByPathUrl = $"{sitecoreUri}{SSCAPIs.ItemApi}{itemId.Trim('{').Trim('}')}/?database=master&language=en";

            var cookies = Authenticate();
            var request = (HttpWebRequest)WebRequest.Create(updateItemByPathUrl);

            request.Method = "PATCH";
            request.ContentType = "application/json";
            request.CookieContainer = cookies;

            var requestBody = JsonConvert.SerializeObject(dataToUpdate);

            var data = new UTF8Encoding().GetBytes(requestBody);

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(data, 0, data.Length);
            }

            var response = request.GetResponse();

            _logger.LogDebug($"Item Status:\n\r{((HttpWebResponse)response).StatusDescription}");
            response.Close();

        }

        private string GetItemPath(string itemId)
		{
            var sitecoreUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");
            var updateItemByPathUrl = $"{sitecoreUri}{SSCAPIs.ItemApi}{itemId.Trim('{').Trim('}')}/?database=master&language=en&fields=ItemPath";

            var cookies = Authenticate();
            var request = (HttpWebRequest)WebRequest.Create(updateItemByPathUrl);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.CookieContainer = cookies;

            var response = request.GetResponse();

            _logger.LogDebug($"Item Status:\n\r{((HttpWebResponse)response).StatusDescription}");

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                var responseFromServer = string.Empty;
                using (var dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    responseFromServer = reader.ReadToEnd();
                }
                GetPerson results = JsonConvert.DeserializeObject<GetPerson>(responseFromServer);
                if(results.ItemPath!=null)
				{
                    response.Close();
                    return results.ItemPath;
                }
            }
            response.Close();
            return string.Empty;
        }

        private string CreateApplicationItem(string firstName, string lastName,string path, string personId, bool updatePersonApplication)
		{

            // Login into Sitecore to authenticate next operation --> create Item 
            var cookies = Authenticate();

            // Use SSC to create application Item
            var sitecoreUri = _configuration.GetValue<string>("Sitecore:InstanceCMUri");

            var createApplication = new CreateApplication
            {
                ItemName = DateTime.Now.Year.ToString(),
                TemplateID = _configuration.GetValue<string>("Sitecore:MVPApplicationTemplateId"),
                FirstName = firstName,
                LastName = lastName
            };

            //TODO - how can we get the path to the user item to pass into the item service API
            var createItemUrl = $"{sitecoreUri}{SSCAPIs.ItemApi}{path}{"?database=master"}"; // Path need to be added to th end ex: https://%3Cdomain%3E/sitecore/api/ssc/item/sitecore%2Fcontent%2Fhome
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
            
            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.Created)
            {
                var createdItemId = response.Headers["Location"].Substring(response.Headers["Location"].LastIndexOf("/"), response.Headers["Location"].Length - response.Headers["Location"].LastIndexOf("/")).Split("?")[0].TrimStart('/');
                if(updatePersonApplication)
				{
                    dynamic dataToUpdate = new
                    {
                        Application = "{" + createdItemId.ToUpper() + "}",
                        Step = ItemsIds.ApplicationSteps.Category
                    };
                    UpdateItemInSc(personId, dataToUpdate);

                }
                response.Close();
                return createdItemId;
            }
            response.Close();
            return null;
        }

        [HttpPost]
        public IActionResult Category(string applicationId, string category)
        {
            try
            {

                dynamic dataToUpdate = new
                {
                    Category = "{" + category.ToUpper() + "}",
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                dataToUpdate = new
                {
                    Application = "{" + applicationId.ToUpper() + "}",
                    Step = ItemsIds.ApplicationSteps.PersonalInformation
                };
                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Category succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }

        [HttpPost]
        public IActionResult PersonalInformation(string applicationId,string firstName, string lastName, string preferredName, string employmentStatus,string companyName, string country, string state, string mentor)
        {
            try
            {
                dynamic dataToUpdate = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PreferredName = preferredName,
                    EmploymentStatus = !string.IsNullOrEmpty(employmentStatus) ? "{" + employmentStatus.ToUpper() + "}" : "",
                    CompanyName = companyName ?? "",
                    Country = !string.IsNullOrEmpty(country) ? "{" + country.ToUpper() + "}" : "",
                    State = state ?? "",
                    Mentor = mentor
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                dataToUpdate = new
                {
                    Application = "{" + applicationId.ToUpper() + "}",
                    Step = ItemsIds.ApplicationSteps.Objectives
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Personal Information succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }

        [HttpPost]
        public IActionResult ObjectivesandMotivation(string applicationId,string eligibility, string objectives)
        {
            try
            {
                dynamic dataToUpdate = new
                {
                    Eligibility = eligibility,
                    Objectives = objectives
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                dataToUpdate = new
                {
                    Application = "{" + applicationId.ToUpper() + "}",
                    Step = ItemsIds.ApplicationSteps.Socials
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Objectives and Motivation succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }

        [HttpPost]
        public IActionResult Socials(string applicationId, string blog, string sitecoreCommunity, string customerCoreProfile, string stackExchange, string gitHub, string twitter, string others)
        {
            try
            {
                dynamic dataToUpdate = new
                {
                    Blog = blog,
                    SitecoreCommunity = sitecoreCommunity,
                    CustomerCoreProfile = customerCoreProfile,
                    StackExchange = stackExchange,
                    GitHub = gitHub,
                    Twitter = twitter,
                    Other =  others
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                dataToUpdate = new
                {
                    Application = "{" + applicationId.ToUpper() + "}",
                    Step = ItemsIds.ApplicationSteps.Contributions
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Socials succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }

        [HttpPost]
        public IActionResult NotableCurrentYearContributions(string applicationId, string onlineAcvitity, string offlineActivity)
        {
            try
            {
                dynamic dataToUpdate = new
                {
                    OnlineAcvitity = onlineAcvitity,
                    OfflineActivity = offlineActivity
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                dataToUpdate = new
                {
                    Application = "{" + applicationId.ToUpper() + "}",
                    Step = ItemsIds.ApplicationSteps.Confirmation
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Notable Current Year Contributions succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }
       
        [HttpPost]
        public IActionResult Confirmation(string applicationId)
        {
            try
            {
                dynamic dataToUpdate = new
                {
                    Completed = "1"
                };

                UpdateItemInSc(applicationId, dataToUpdate);

                return Json(new { success = true, responseText = "Category succesffuly updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return Json(new { success = false, responseText = "An error occured while saving your application.  Please contact Sitecore Support." });
            }
        }

        private CookieContainer Authenticate()
        {
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


            return cookies;
        }
    }
}
