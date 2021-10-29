using Mvp.Feature.Forms.Models;
using Mvp.Feature.Forms.Search;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms
{
    public class Helper
    {
        internal class SessionConstants
        {
            internal const string UserApplicationId = "UserApplicationId";
        }

        public static SearchResults<PeopleDataItem> SearchPeopleByEmail(string email)
        {
            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(email, "email can't be null or empty");

            var peopleFolder = Sitecore.Context.Database.GetItem(Constants.Person.Folder.FOLDER_ID);
            Sitecore.Diagnostics.Assert.IsNotNull(peopleFolder, "can't find people folder");


            var indexable = new SitecoreIndexableItem(peopleFolder);
            using (var context = ContentSearchManager.GetIndex(indexable).CreateSearchContext())
            {
                //original search query conditions that pull back blog items with tags in common
                var searchQuery = context.GetQueryable<PeopleDataItem>().Where(x => x.TemplateId == Constants.Person.Template.TEMPLATE_ID && x.Paths.Contains(peopleFolder.ID));
                var predicate = PredicateBuilder.True<PeopleDataItem>();
                predicate = predicate.And(x => x.Email == email);
                searchQuery = searchQuery.Where(predicate);
                return searchQuery.GetResults();
            }
        }

        public static SearchResults<PeopleDataItem> SearchPeopleByOktaId(string identifier)
        {
            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(identifier, "identifier can't be null or empty");

            var peopleFolder = Sitecore.Context.Database.GetItem(Constants.Person.Folder.FOLDER_ID);
            Sitecore.Diagnostics.Assert.IsNotNull(peopleFolder, "can't find people folder");

            var indexable = new SitecoreIndexableItem(peopleFolder);
            using (var context = ContentSearchManager.GetIndex(indexable).CreateSearchContext())
            {
                //original search query conditions that pull back blog items with tags in common
                var searchQuery = context.GetQueryable<PeopleDataItem>().Where(x => x.TemplateId == Constants.Person.Template.TEMPLATE_ID && x.Paths.Contains(peopleFolder.ID));
                var predicate = PredicateBuilder.True<PeopleDataItem>();
                predicate = predicate.And(x => x.OktaId == identifier);
                searchQuery = searchQuery.Where(predicate);
                return searchQuery.GetResults();
            }
        }


        public static Application GetApplicationModel(string applicationItemId)
        {
            Item applicationItem = Sitecore.Context.Database.GetItem(applicationItemId);

            if (applicationItem == null)
                return null;

            var application = new Application()
            {
                ApplicationId = applicationItem.ID.ToString().TrimStart('{').TrimEnd('}'),
                OfficialFirstName = applicationItem.Fields[Constants.Application.Template.Fields.OFFICIAL_FIRST_NAME].Value,
                OfficialLastName = applicationItem.Fields[Constants.Application.Template.Fields.OFFICIAL_LAST_NAME].Value,
                PreferredName = applicationItem.Fields[Constants.Application.Template.Fields.PREFERRED_NAME].Value,
                CompanyName = applicationItem.Fields[Constants.Application.Template.Fields.COMPANY_NAME].Value,
                Mentor = applicationItem.Fields[Constants.Application.Template.Fields.MENTOR].Value,
                Eligibility = applicationItem.Fields[Constants.Application.Template.Fields.ELIGIBILITY].Value,
                Objectives = applicationItem.Fields[Constants.Application.Template.Fields.OBJECTIVES].Value,
                Blog = applicationItem.Fields[Constants.Application.Template.Fields.BLOG].Value,
                SitecoreCommunity = applicationItem.Fields[Constants.Application.Template.Fields.SITECORE_COMMUNITY].Value,
                CustomerCoreProfile = applicationItem.Fields[Constants.Application.Template.Fields.CUSTOMER_CORE_PROFILE].Value,
                StackExchange = applicationItem.Fields[Constants.Application.Template.Fields.STACK_EXCHANGE].Value,
                GitHub = applicationItem.Fields[Constants.Application.Template.Fields.GITHUB].Value,
                Twitter = applicationItem.Fields[Constants.Application.Template.Fields.TWITTER].Value,
                Other = applicationItem.Fields[Constants.Application.Template.Fields.OTHER].Value,
                OnlineActivity = applicationItem.Fields[Constants.Application.Template.Fields.ONLINE_ACTIVITY].Value,
                OfflineActivity = applicationItem.Fields[Constants.Application.Template.Fields.OFFLINE_ACTIVITY].Value,

                MVPCategory = GetMVPCategoryModel(applicationItem.Fields[Constants.Application.Template.Fields.CATEGORY].Value),
                Country = GetCountryModel(applicationItem.Fields[Constants.Application.Template.Fields.COUNTRY].Value),
                // State = GetStateModel(applicationItem.Fields[Constants.Application.Template.Fields.STATE].Value),
                EmploymentStatus = GetEmploymentStatusModel(applicationItem.Fields[Constants.Application.Template.Fields.EMPLOYMENT_STATUS].Value),
            };

         

            return application;

        }

        public static MVPCategory GetMVPCategoryModel(string mvpCategoryItemId)
        {
            if (string.IsNullOrWhiteSpace(mvpCategoryItemId))
                return null;

            Item MVPCategoryItem = Sitecore.Context.Database.GetItem(mvpCategoryItemId);

            if (MVPCategoryItem == null)
                return null;

            CheckboxField ck = MVPCategoryItem.Fields[Constants.MVPCategory.Template.Fields.ACTIVE];
            var mvpCategoryModel = new MVPCategory()
            {
                Name = MVPCategoryItem.Fields[Constants.MVPCategory.Template.Fields.NAME].Value,
                Active = ck?.Checked??false,
            };

            return mvpCategoryModel;

        }
        public static Country GetCountryModel(string countryItemId)
        {
            if (string.IsNullOrWhiteSpace(countryItemId))
                return null;

            Item countryItem = Sitecore.Context.Database.GetItem(countryItemId);

            if (countryItem == null)
                return null;

            var country = new Country()
            {
                Name = countryItem.Fields[Constants.Country.Template.Fields.NAME].Value,
                Description = countryItem.Fields[Constants.Country.Template.Fields.DESCRIPTION].Value,
            };

            return country;

        
        }

        

        public static ApplicationStep GetApplicationStepModel(string applicationStepItemId)
        {
            if (string.IsNullOrWhiteSpace(applicationStepItemId))
                return null;

            Item Item = Sitecore.Context.Database.GetItem(applicationStepItemId);

            if (Item == null)
                return null;

            var applicationStep = new ApplicationStep()
            {
                StepId = Item.Fields[Constants.ApplicationStep.Template.Fields.APPLICATION_STEP_ID].Value,
                Title = Item.Fields[Constants.ApplicationStep.Template.Fields.APPLICATION_STEP_TITLE].Value,
            };

            return applicationStep;
            
        }


        public static EmploymentStatus GetEmploymentStatusModel(string employmentStatusItemId)
        {
            Item employmentStatusItem = Sitecore.Context.Database.GetItem(employmentStatusItemId);

            if (employmentStatusItem == null)
                return null;

            
            var employmentStatus = new EmploymentStatus()
            {
                Name = employmentStatusItem.Fields[Constants.EmploymentStatus.Template.Fields.NAME].Value,
                Description = employmentStatusItem.Fields[Constants.EmploymentStatus.Template.Fields.DESCRIPTION].Value,
            };

            return employmentStatus;

        }

        public static IEnumerable<Country> GetCountries()
        {
            //to do: it should be cached

            Item FolderItem = Sitecore.Context.Database.GetItem(Constants.Country.Folder.FOLDER_ID);

            if (FolderItem == null)
                yield break;

            foreach (Item country  in FolderItem.Children)
            {
                var countryModel = new Country { 
                    Name = country.Fields[Constants.Country.Template.Fields.NAME].Value,
                    Description = country.Fields[Constants.Country.Template.Fields.DESCRIPTION].Value,
                };

                if (string.IsNullOrWhiteSpace(countryModel.Name))
                    countryModel.Name = country.Name;

                yield return countryModel;
            }

        }

        public static IEnumerable<MVPCategory> GetMVPCategories()
        {
            //to do: it should be cached

            Item FolderItem = Sitecore.Context.Database.GetItem(Constants.MVPCategory.Folder.FOLDER_ID);

            if (FolderItem == null)
                yield break;

            foreach (Item mvpCategory in FolderItem.Children)
            {
                CheckboxField ck = mvpCategory.Fields[Constants.MVPCategory.Template.Fields.ACTIVE];
 
                var mvpCategoryModel = new MVPCategory
                {
                    Name = mvpCategory.Fields[Constants.MVPCategory.Template.Fields.NAME].Value,
                    Active = ck?.Checked ?? false,
                };

                if (string.IsNullOrWhiteSpace(mvpCategoryModel.Name))
                    mvpCategoryModel.Name = mvpCategory.Name;

                yield return mvpCategoryModel;
            }

        }

        public static IEnumerable<EmploymentStatus> GetEmploymentStatus()
        {
            //to do: it should be cached

            Item FolderItem = Sitecore.Context.Database.GetItem(Constants.EmploymentStatus.Folder.FOLDER_ID);

            if (FolderItem == null)
                yield break;

            foreach (Item employmentStatus in FolderItem.Children)
            {
                var employmentStatusModel = new EmploymentStatus
                {
                    Name = employmentStatus.Fields[Constants.EmploymentStatus.Template.Fields.NAME].Value,
                    Description = employmentStatus.Fields[Constants.EmploymentStatus.Template.Fields.DESCRIPTION].Value,
                };

                if (string.IsNullOrWhiteSpace(employmentStatusModel.Name))
                    employmentStatusModel.Name = employmentStatus.Name;

                yield return employmentStatusModel;
            }

        }

        /*
        public static State GetStateModel(string stateItemId)
        {
            Item stateItem = Sitecore.Context.Database.GetItem(stateItemId);

            if (stateItem == null)
                return null;

            CheckboxField ck = stateItem.Fields[Constants.State.Template.Fields.ACTIVE];
            var state = new State()
            {
                Name = stateItem.Fields[Constants.State.Template.Fields.NAME].Value,
                Description = stateItem.Fields[Constants.State.Template.Fields.DESCRIPTION].Value,
            };

            return state;
        }
        */

    }
}