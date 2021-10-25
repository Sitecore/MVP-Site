using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Sitecore.Abstractions;
using Sitecore.Owin.Authentication.Configuration;
using Sitecore.Owin.Authentication.Extensions;
using Sitecore.Owin.Authentication.Identity;
using Sitecore.Owin.Authentication.Pipelines.IdentityProviders;
using Sitecore.Owin.Authentication.Services;
using Sitecore.Owin.Extensions;

namespace Mvp.Foundation.User.Pipelines.IdentityProviders
{
    public class HeadlessOktaIdentityProvider : IdentityProvidersProcessor
    {
        /*
         <setting name="Okta_OktaDomain" value="https://dev-45952888.okta.com" />
            <setting name="Okta_ClientId" value="0oa1jhwqy63SWS4sG5d7" />
            <setting name="Okta_ClientSecret" value="KrF9WOnKRCpYsQEjm-RPr9kJ-NjEHWpJBRyfpiUj" />
            <setting name="Okta_AuthorizationServerId" value="default" />
         */

        // App config settings
        private static readonly string ClientId = Sitecore.Configuration.Settings.GetSetting("Okta_ClientId");
        //private static readonly string ClientSecret = Sitecore.Configuration.Settings.GetSetting("Okta_ClientSecret");
        private static readonly string Authority = Sitecore.Configuration.Settings.GetSetting("Okta_OktaDomain");
        private static readonly string AuthorizationServerId = Sitecore.Configuration.Settings.GetSetting("Okta_AuthorizationServerId");
        //private static readonly string OAuthRedirectUri = Sitecore.Configuration.Settings.GetSetting("Okta_RedirectUri");

        // constant settings
        private static readonly string OauthMetaDataEndpoint = $"{Authority}/oauth2/{AuthorizationServerId}/.well-known/openid-configuration";
        //private const string OauthTokenEndpoint = "/oauth2/v1/token";
        //private const string OauthUserInfoEndpoint = "/oauth2/v1/userinfo";
        //private const string OpenIdScope = OpenIdConnectScope.OpenIdProfile + " email";

        protected ApplicationUserFactory ApplicationUserFactory { get; }

        public HeadlessOktaIdentityProvider(
            FederatedAuthenticationConfiguration federatedAuthenticationConfiguration,
            ICookieManager cookieManager,
            BaseSettings settings,
            ApplicationUserFactory applicationUserFactory)
            : base(federatedAuthenticationConfiguration, cookieManager, settings)
        {
            this.ApplicationUserFactory = applicationUserFactory;
        }

        protected override string IdentityProviderName => "headless.okta";

        protected override void ProcessCore(IdentityProvidersArgs args)
        {
            //DO NOT ENABLE THIS FOR PROD, DEBUG ONLY
            IdentityModelEventSource.ShowPII = true;

            var identityProvider = this.GetIdentityProvider();
            var authenticationType = this.GetAuthenticationType();

            var tvps = new TokenValidationParameters
            {
                AuthenticationType = identityProvider.Name,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidAudience = ClientId,

                // This line is critical:
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
            };

            args.App.UseOAuthBearerAuthentication(
                new OAuthBearerAuthenticationOptions
                {
                    AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(OauthMetaDataEndpoint)),

                    Provider = new OAuthBearerAuthenticationProvider()
                    {
                        OnRequestToken = (context) =>
                        {
                            return Task.CompletedTask;
                        },
                        OnValidateIdentity = (context) =>
                        {
                            var identity = context.Ticket.Identity;

                            foreach (var claimTransformationService in identityProvider.Transformations)
                            {
                                claimTransformationService.Transform(identity,
                                new TransformationContext(FederatedAuthenticationConfiguration, identityProvider));

                            }

                            context.Validated(new AuthenticationTicket(identity, context.Ticket.Properties));

                            //var loginInfo = new ExternalLoginInfo
                            //{
                            //    ExternalIdentity = identity,
                            //    DefaultUserName = identity.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"),
                            //    Email = identity.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                            //};

                            //var resultTask = Task.Run(async () => await this.SignInAsync(loginInfo, context.OwinContext));
                            //Task.WaitAll(resultTask);

                            return Task.CompletedTask;
                        }
                    }
                });
        }

        //protected virtual async Task SignInAsync(ExternalLoginInfo loginInfo, IOwinContext context)
        //{
        //    ApplicationUser applicationUser = this.ApplicationUserFactory.CreateUser(loginInfo.DefaultUserName);
        //    UserManager<ApplicationUser> userManager = context.GetUserManager();

        //    applicationUser.IsVirtual = true;
        //    ConfiguredTaskAwaitable<IdentityResult> configuredTaskAwaitable1 = userManager.CreateAsync(applicationUser).ConfigureAwait(false);
        //    await configuredTaskAwaitable1;

        //    using (TemporaryClaimsStorage temporaryClaimsStorage = TemporaryClaimsStorage.Init(loginInfo.ExternalIdentity, context.GetHttpContext()))
        //    {
        //        SignInManager<ApplicationUser, string> signInManager = context.GetSignInManager();
        //        ConfiguredTaskAwaitable configuredTaskAwaitable2 = signInManager.SignInAsync(applicationUser, false, false).ConfigureAwait(false);
        //        await configuredTaskAwaitable2;
        //    }
        //}
    }
 
}