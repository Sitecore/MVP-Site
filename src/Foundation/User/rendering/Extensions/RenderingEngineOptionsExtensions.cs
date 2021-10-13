using Sitecore.AspNet.RenderingEngine.Configuration;
using Sitecore.AspNet.RenderingEngine.Extensions;

namespace Mvp.Foundation.User.Extensions
{
    public static class RenderingEngineOptionsExtensions
    {
        public static RenderingEngineOptions AddFoundationUser(this RenderingEngineOptions options)
        {
            options.AddPartialView("SignIn");

            return options;
        }
    }
}