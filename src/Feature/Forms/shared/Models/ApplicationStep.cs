using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvp.Feature.Forms.Shared.Models
{
    public class MVPCategory
    {
        public string Name { get; set; }
        public bool Active{ get; set; }
        public Guid ID { get; set; }
    }
}