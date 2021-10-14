using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Models
{
    public class ApplicationInfo
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public string ApplicationStep { get; set; }
        public Guid Application { get; set; }
    }
}