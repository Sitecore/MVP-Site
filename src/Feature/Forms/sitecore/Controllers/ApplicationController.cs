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

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : SitecoreController
    {
        [HttpPost]
        public JsonResult GetApplicationInfo(string identifier)
        {
            //TODO:need to pass identifier from the currently logged in user
            if (string.IsNullOrEmpty(identifier))
                return null;

            var peopleFolder = Sitecore.Context.Database.GetItem(Constants.People.Folder.FOLDER_ID);
            var searchResults = SearchPeople(peopleFolder, identifier);
            
            if (searchResults != null && searchResults.Any())
            {
                var person = searchResults.FirstOrDefault();
                var personItem  = Sitecore.Context.Database.GetItem(person.Document.ItemId);

                var result = new ApplicationInfo()
                {
                    FirstName = personItem.Fields[Constants.People.Fields.PEOPLE_FIRST_NAME].Value,
                    LastName = personItem.Fields[Constants.People.Fields.PEOPLE_LAST_NAME].Value,
                };

                //Set Application Info for result
                var applicationStep = personItem.Fields[Constants.People.Fields.PEOPLE_APPLICATION_STEP].Value;
                var applicationStepItem = Sitecore.Context.Database.GetItem(new ID(applicationStep));

                result.ApplicationStep = applicationStepItem.Fields[Constants.ApplicationStep.Fields.APPLICATION_STEP_VIEW_PATH].Value;
                return Json(result);
            }

            return Json(new { result = false, error = "Applicable person not found." });

        }

        private SearchResults<PeopleDataItem> SearchPeople(Item sourceItem, string identifier)
        {
            var indexable = new SitecoreIndexableItem(sourceItem);
            using (var context = ContentSearchManager.GetIndex(indexable).CreateSearchContext())
            {
                //original search query conditions that pull back blog items with tags in common
                var searchQuery = context.GetQueryable<PeopleDataItem>().Where(x => x.TemplateId == Constants.People.Template.TEMPLATE_ID && x.Paths.Contains(sourceItem.ID));
                var predicate = PredicateBuilder.True<PeopleDataItem>();
                predicate = predicate.And(x => x.Email.Contains(identifier));
                searchQuery = searchQuery.Where(predicate);
                return searchQuery.GetResults();
            }
        }
    }
}