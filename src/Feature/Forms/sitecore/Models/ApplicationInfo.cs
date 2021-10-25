using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Models
{
    public class ApplicationInfo
    {
        
        public string ApplicationStep { get; set; }
        public Application Application { get; set; }
   
      //  public Person Person { get; set; }
    }

    public class ApplicationLists {
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<EmploymentStatus> EmploymentStatus { get; set; }
        public IEnumerable<MVPCategory> MVPCategories { get; set; }
    }
}