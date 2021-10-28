using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Forms.Models
{
    public class ApplicationEditableFields
    {
        public class CategoryModel
        {
            public string Category { get; set; }
        }

        public class PersonalInformationModel
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string PreferredName { get; set; }

            public string EmploymentStatus { get; set; }

            public string CompanyName { get; set; }

            public string Country { get; set; }

            public string State { get; set; }

            public string Mentor { get; set; }
        }

        public class ObjectivesandMotivationModel
        {
            public string Eligibility { get; set; }

            public string Objectives { get; set; }
        }


        public class CommunityProfilesModel
        {
            public string Blog { get; set; }

            public string SitecoreCommunity { get; set; }

            public string CustomerCoreProfile { get; set; }

            public string StackExchange { get; set; }

            public string GitHub { get; set; }

            public string Twitter { get; set; }

            public string Others { get; set; }
        }

        public class NotableCurrentYearContributionsModel
        {
            public string OnlineAcvitity { get; set; }

            public string OfflineActivity { get; set; }
        }
    }
}
