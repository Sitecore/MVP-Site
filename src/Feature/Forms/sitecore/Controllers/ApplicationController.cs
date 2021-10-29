using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Mvp.Feature.Forms.Search;
using Mvp.Feature.Forms.Models;
using Sitecore.Data.Fields;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : Controller
    {
        [HttpGet]
        public JsonResult GetApplicationInfo()
        {
            if (Sitecore.Context.User.Identity.IsAuthenticated) 
            {
                
                var identifier = ((System.Security.Claims.ClaimsIdentity)Sitecore.Context.User.Identity).FindFirst("aud").Value;
                
                var searchResults = Helper.SearchPeopleByOktaId( identifier);

                //fallback to email verification assuming the persons okta id was updated, can be removed later
                if (searchResults == null || !searchResults.Any()){
                    var email = ((System.Security.Claims.ClaimsIdentity)Sitecore.Context.User.Identity).FindFirst("email").Value;
                    searchResults = Helper.SearchPeopleByEmail(email);
                }

                if (searchResults != null && searchResults.Any())
                {
                    var person = searchResults.FirstOrDefault();
                    var personItem = Sitecore.Context.Database.GetItem(person.Document.ItemId);

                    if (personItem != null)
                    {
                        var applicationStepId = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_APPLICATION_STEP].Value;
                        ApplicationStep applicationStep = Helper.GetApplicationStepModel(applicationStepId);

                        var applicationItemId= personItem.Fields[Constants.Person.Template.Fields.PEOPLE_APPLICATION]?.Value;
                        var applicationModel= Helper.GetApplicationModel(applicationItemId);
                      
                        if (applicationModel != null) {
                            var applicationInfoModel = new ApplicationInfo
                            {
                                Application = applicationModel,
                                ApplicationStep = applicationStep
                            };
 
                            return Json(applicationInfoModel, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { result = true, error = "application not found."  }, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json(new { result = false, error = "person not found." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result = false, error = "Please login first." }, JsonRequestBehavior.AllowGet);
        
        }


        [HttpGet]
        public JsonResult GetApplicationLists()
        {
            if (Sitecore.Context.User.Identity.IsAuthenticated) 
            {
                 var applicationListsModel = new ApplicationLists{
                                Countries  = Helper.GetCountries(),
                                EmploymentStatus = Helper.GetEmploymentStatus(),
                                MVPCategories = Helper.GetMVPCategories(),
                        };

                    return Json(applicationListsModel, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false, error = "please signin to get the application lists." }, JsonRequestBehavior.AllowGet);

        }

    }
}
 