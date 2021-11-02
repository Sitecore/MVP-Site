using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Shared.Models
{
    public class Country
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ID { get; set; }
    }
}