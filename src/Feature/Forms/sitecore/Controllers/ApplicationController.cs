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
using Mvp.Feature.Forms.Shared.Models;
using Sitecore.Data.Fields;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : Controller
    {
        FormsService _service;
        public ApplicationController()
		{
            _service = new FormsService();
		}

        [HttpPost]
        public JsonResult GetApplicationInfo(string identifier, string email)
		{

			var applicationInfoModel = new ApplicationInfo();
			if (string.IsNullOrEmpty(identifier) && string.IsNullOrEmpty(email))
			{
				applicationInfoModel = new ApplicationInfo
				{
					Status = ApplicationStatus.PersonItemNotFound
				};

				return Json(applicationInfoModel, JsonRequestBehavior.AllowGet);
			}

            if(!string.IsNullOrEmpty(identifier)) { 

				var personItem = _service.SearchPeopleByOktaId(identifier);

				//fallback to email verification assuming the persons okta id was updated, can be removed later
				if (personItem == null)
				{
					personItem = _service.SearchPeopleByEmail(email);
				}

				if (personItem != null)
				{
					Person personO = new Person();
					personO.FirstName = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_FIRST_NAME].Value;
					personO.LastName = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_LAST_NAME].Value;
					personO.OktaId = personItem.Fields[Constants.Person.Template.Fields.OKTA_ID].Value;
					personO.Email = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_EMAIL].Value;
					personO.ItemPath = personItem.Paths.FullPath;
					personO.ItemId = personItem.ID.ToString();
					

					var applicationItemId = personItem.Fields[Constants.Person.Template.Fields.PEOPLE_APPLICATION]?.Value;
					var applicationModel = _service.GetApplicationModel(applicationItemId);

					if (applicationModel != null)
					{
						var applicationStepId = applicationModel.Step;
						ApplicationStep applicationStep = _service.GetApplicationStepModel(applicationStepId);

						applicationInfoModel = new ApplicationInfo
						{
							Application = applicationModel,
							ApplicationStep = applicationStep,
							Person = personO,
							Status = ApplicationStatus.ApplicationFound
						};
					}
					else
					{
						applicationInfoModel = new ApplicationInfo
						{
							Person = personO,
							Status = ApplicationStatus.ApplicationItemNotFound
						};
					}
				}
				else
				{
					applicationInfoModel = new ApplicationInfo
					{
						Status = ApplicationStatus.PersonItemNotFound
					};
				}
			}
			else	
			{
				applicationInfoModel = new ApplicationInfo
				{
					Status = ApplicationStatus.NotLoggedIn
				};
			}
			return Json(applicationInfoModel, JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
        public JsonResult GetApplicationLists()
        {
            {
                 var applicationListsModel = new ApplicationLists{
                                Country  = _service.GetCountries(),
                                EmploymentStatus = _service.GetEmploymentStatus(),
                                MVPCategories = _service.GetMVPCategories(),
                        };

                    return Json(applicationListsModel, JsonRequestBehavior.AllowGet);
            }

        }

    }
}
 