using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mvp.Foundation.User.Extensions
{
    public static class UserHtmlHelper
    {
        public const string EmailClaim = "email";
        public const string UniqueIdClaim = "idp"; // This may change when integrated with real Okta account

        public static string GetUserEmail(this IHtmlHelper htmlHelper)
        {
            return GetUserClaimValue(htmlHelper, EmailClaim) ?? string.Empty;
        }

        public static string GetUserId(this IHtmlHelper htmlHelper)
        {
            return GetUserClaimValue(htmlHelper, UniqueIdClaim) ?? string.Empty;
        }

        // ============================================================================================

        private static string GetUserClaimValue(IHtmlHelper htmlHelper, string claim)
        {
            var user = htmlHelper.ViewContext.HttpContext.User;
            var identity = (ClaimsIdentity)user?.Identity;
            return identity?.FindFirst(claim)?.Value;
        }

    }
}