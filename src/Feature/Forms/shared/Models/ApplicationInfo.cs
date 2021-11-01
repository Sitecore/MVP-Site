using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Shared.Models
{
    public class ApplicationInfo
    {
        
        public ApplicationStep ApplicationStep { get; set; }
        public Application Application { get; set; }
        public Person Person { get; set; }

        public ApplicationStatus Status { get; set; }
    }

    public enum ApplicationStatus
	{
        NotLoggedIn = -1,
        PersonItemNotFound = 0,
        ApplicationItemNotFound=1,
        ApplicationFound =2
	}
    public class ApplicationLists {
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<EmploymentStatus> EmploymentStatus { get; set; }
        public IEnumerable<MVPCategory> MVPCategories { get; set; }
    }
}