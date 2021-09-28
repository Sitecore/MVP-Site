using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNet.RenderingEngine.Binding;
namespace Mvp.Foundation.Users.Components
{
    public class SignInComponent : ViewComponent
    {
        private readonly IViewModelBinder modelBinder;

        // Use the view component support for dependency injection to inject
        // FakeService and the
        // Sitecore.AspNet.RenderingEngine.Binding.IViewModelBinder service.
        public SignInComponent(IViewModelBinder modelBinder)
        {
            this.modelBinder = modelBinder;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Bind MyServiceModel to the Sitecore Layout Service response
            // when the view component is invoked.
            // var model = await modelBinder.Bind<MyModel>(this.ViewContext);

            // var isAuthenticated = this.HttpContext.User.Identity.IsAuthenticated;

            // Return the model to the Default.cshtml Razor view.
            return View();
        }
    }
}