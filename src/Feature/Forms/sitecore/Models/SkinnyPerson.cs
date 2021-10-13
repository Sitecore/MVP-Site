using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Models
{
    public class SkinnyPerson
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Guid ApplicationStep { get; set; }
        public Guid Application { get; set; }
    }
}