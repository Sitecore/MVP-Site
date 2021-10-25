using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Introduction { get; set; }
        public Country Country { get; set; }
        public string Awards { get; set; }//item
        public string Application { get; set; }//item
        public string Step { get; set; }//item
        public string OktaId { get; set; }

    }
}