using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mvp.Project.MvpSite.Models;
using Sitecore.AspNet.RenderingEngine;
using Sitecore.AspNet.RenderingEngine.Filters;
using Sitecore.LayoutService.Client.Exceptions;
using Sitecore.LayoutService.Client.Response.Model;
using System.Net;

namespace Mvp.Project.MvpSite.Controllers
{
    public class DefaultController : Controller
    {

        public DefaultController()
        {

        }

        // Inject Sitecore rendering middleware for this controller action
        // (enables model binding to Sitecore objects such as Route,
        // and causes requests to the Sitecore Layout Service for controller actions)
        [UseSitecoreRendering]
        public IActionResult Index(Route route)
        {
            var request = HttpContext.GetSitecoreRenderingContext();

            if (request.Response.HasErrors)
            {
                foreach (var error in request.Response.Errors)
                {
                    switch (error)
                    {
                        case ItemNotFoundSitecoreLayoutServiceClientException notFound:
                            Response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        case InvalidRequestSitecoreLayoutServiceClientException badRequest:
                        case CouldNotContactSitecoreLayoutServiceClientException transportError:
                        case InvalidResponseSitecoreLayoutServiceClientException serverError:
                        default:
                            throw error;
                    }
                }
            }

            return View(route);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            return View(new ErrorViewModel
            {
                IsInvalidRequest = exceptionHandlerPathFeature?.Error is InvalidRequestSitecoreLayoutServiceClientException
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Healthz()
        {
            // TODO: Do we want to add logic here to confirm connectivity with SC etc?

            return Ok("Healthy");
        }
    }
}