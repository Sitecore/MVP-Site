using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : SitecoreController
    {
        [HttpPost]
        public JsonResult GetUserApplicationStep(string identifier)
        {
            //Don't execute for anything less than 2.
            if (string.IsNullOrEmpty(identifier))
                return null;

            
            return Json("test");
        }
    }
}