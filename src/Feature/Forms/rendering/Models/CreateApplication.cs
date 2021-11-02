using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Feature.Forms.Models
{
    public class CreateApplication: CreateBase
    {
        public string FirstName { get;set; }
        public string LastName { get; set; }
    }
}
