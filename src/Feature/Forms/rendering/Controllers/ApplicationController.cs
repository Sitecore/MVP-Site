using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Mvp.Feature.Forms.Models;

namespace Mvp.Feature.Forms.Controllers
{
    public class ApplicationController : Controller
    {
        [HttpPost]
        public IActionResult SubmitStep(string input)
        {
            var appModel = new ApplicationModel();
            return Json(appModel);
        }
    }
}
