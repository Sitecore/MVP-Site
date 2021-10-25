using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Models
{
    public class Application
    {
        public MVPCategory MVPCategory { get; set; }
        public string OfficialFirstName { get; set; }
        public string OfficialLastName { get; set; }
        public string PreferredName { get; set; }
        public EmploymentStatus EmploymentStatus { get; set; }
        public string CompanyName { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
        public string Mentor { get; set; }
        public string Eligibility { get; set; }
        public string Objectives { get; set; }
        public string Blog { get; set; }
        public string SitecoreCommunity { get; set; }
        public string CustomerCoreProfile { get; set; }
        public string StackExchange { get; set; }
        public string GitHub { get; set; }
        public string Twitter { get; set; }
        public string Other { get; set; }
        public string OnlineActivity { get; set; }
        public string OfflineActivity { get; set; }
 
    }
}