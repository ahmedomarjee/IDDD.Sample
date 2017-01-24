using System;
using System.Net.Http;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using IDDD.App;
using AspNet.Security.OAuth.Validation;

namespace IDDD.Web.Api.Infrastructure
{
    public static class AuthenticationConfiguration
    {
        public static IApplicationBuilder ConfigureAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            app.UseWhen(IsApi, ApiAuthentication(configuration));
            app.UseOpenIdConnectServer(ServerOptions);
            return app;
        }

        private static bool IsApi(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(new PathString("/api"));
        }

        private static Action<IApplicationBuilder> ApiAuthentication(IConfiguration configuration)
        {
            var options = new OAuthValidationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            };

            options.Audiences.Add(configuration.ApiHostName());

            return branch => branch.UseOAuthValidation(options);
        }

        private static void ServerOptions(OpenIdConnectServerOptions options)
        {
            options.Provider = new AuthorizationServerProvider();
            options.AllowInsecureHttp = true;
            // Enable endpoints.
            options.TokenEndpointPath = "/connect/token";
            options.RevocationEndpointPath = "/connect/revoke";

            options.AccessTokenLifetime = TimeSpan.FromMinutes(15);
            options.RefreshTokenLifetime = TimeSpan.FromMinutes(45);

            // Note: see AuthorizationController.cs for more
            // information concerning ApplicationCanDisplayErrors.
            options.ApplicationCanDisplayErrors = true;
            options.AllowInsecureHttp = true;

        }
    }
}
