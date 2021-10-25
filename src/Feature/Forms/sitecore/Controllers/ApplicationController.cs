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
    public class ApplicationController : SitecoreController
    {
        [HttpGet]
        public JsonResult GetApplicationInfo()
        {
            //TODO:need to pass identifier from the currently logged in user

            //if (Sitecore.Context.IsLoggedIn && Sitecore.Context.User.Identity.IsAuthenticated) 
            if (true) //just for dev
            {
                var identifier = "123456m";// Sitecore.Context.User.Identity.Name;
                
                var searchResults = Helper.SearchPeopleByOktaId( identifier);

                if (searchResults != null && searchResults.Any())
                {
                    var person = searchResults.FirstOrDefault();
                    var personItem = Sitecore.Context.Database.GetItem(person.Document.ItemId);

                    if (personItem != null)
                    {
                        //Set Application Info for result
                        var applicationStep = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_APPLICATION_STEP].Value;
                        var applicationStepItem = Sitecore.Context.Database.GetItem(new ID(applicationStep));

                        var applicationItemId= personItem.Fields[Constants.Person.Template.Fields.PEOPLE_APPLICATION]?.Value;
                        var applicationModel= Helper.GetApplicationModel(applicationItemId);
                      
                        if (applicationModel != null) {
                            var applicationInfoModel = new ApplicationInfo
                            {
                                Application = applicationModel,
                                ApplicationStep = applicationStepItem.Fields[Constants.ApplicationStep.Fields.APPLICATION_STEP_VIEW_PATH].Value
                            };

                            return Json(applicationInfoModel, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { result = false, error = "application can't be found." }, JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json(new { result = false, error = "person can't be found." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result = false, error = "Please login first." }, JsonRequestBehavior.AllowGet);
        
        }


        [HttpGet]
        public JsonResult GetApplicationLists()
        {
            //TODO:need to pass identifier from the currently logged in user

            //if (Sitecore.Context.IsLoggedIn && Sitecore.Context.User.Identity.IsAuthenticated) 
            if (true) //just for dev
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
 