using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.Extensions.Configuration;
using AuthenticationProperties = Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties;
using IDDD.App.Cqs.Queries.Clients;
using IDDD.App.Cqs.Queries.Users;
using IDDD.App.Cqs.QueryResult.Users;
using IDDD.App.Cqs.Queries.RefreshTokens;
using IDDD.App.Cqs.Commands.RefreshTokens;
using IDDD.Core.Cqs.Query;
using IDDD.Core.Cqs.Command;
using IDDD.App.Cqs;
using IDDD.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using IDDD.Domain.Membership;

namespace IDDD.Web.Api.Infrastructure
{
    public partial class AuthorizationServerProvider : OpenIdConnectServerProvider
    {
        /// <summary>
        /// Validates whether the client is a valid known application in our system.
        /// </summary>
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {   


            if (!context.Request.IsRefreshTokenGrantType()
                && !context.Request.IsPasswordGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only password and refresh token grant types " +
                                 "are accepted by this authorization server");

                return;
            }

            var query = new ValidateClientQuery(context.ClientId, context.ClientSecret);
            var result = await ExecuteQuery(context, query);

            if (!result.Succeeded)
            {
                context.Reject(
                    error: "invalid_client",
                    description: "Client not found in the database: ensure that your client_id is correct");

                return;
            }

            context.HttpContext.Items.Add("as:clientAllowedOrigin", result.AllowedOrigin);
            if (context.Request.IsPasswordGrantType())
            {
                // Resolve ASP.NET Core Identity's user manager from the DI container.
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByNameAsync(context.Request.Username);

                if (user == null)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid user details.");
                    return;
                }


                // Reject the token request if email confirmation is required.
                //if (!(await userManager.IsEmailConfirmedAsync(user)))
                //{
                //    context.Reject(
                //        error: OpenIdConnectConstants.Errors.InvalidGrant,
                //        description: "Email confirmation is required for this account.");

                //    return;
                //}
            }

            context.Validate();
        }

        
        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (!context.Request.IsPasswordGrantType()
                && !context.Request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only password and refresh token grant types " +
                                 "are accepted by this authorization server");

                return;
            }

            if (context.Request.IsPasswordGrantType())
            {
                await this.GrantResourceOwnerCredentials(context);
            }
            if (context.Request.IsRefreshTokenGrantType())
            {
                await this.GrantRefreshToken(context);
            }
        }
        

        /// <summary>
        /// Validates the userName and password provided by the user.
        /// </summary>
        private async Task GrantResourceOwnerCredentials(HandleTokenRequestContext context)
        {
            var query = new UserNamePasswordLoginQuery(context.Request.Username, context.Request.Password);
            var result = await ExecuteQuery(context, query);

            if (!result.Succeeded)
            {
                context.Reject("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            SetCorsHeader(context);

            var ticket = CreateAuthenticationTicket(result, context);
            context.Validate(ticket);
        }

        /// <summary>
        /// Grant a new access_token based on the current refresh_token. Here we couldvalidate whether the 
        /// refresh token is still valid or revoked.
        /// </summary>
        private async Task GrantRefreshToken(HandleTokenRequestContext context)
        {
            var validator = new RefreshTokenValidatorQuery(
                context.Ticket.GetTicketId(),
                context.Request.ClientId,
                context.Ticket.Principal.GetClaim(ClaimTypes.NameIdentifier));

            var result = await ExecuteQuery(context, validator);
            if (!result.Succeeded)
            {
                context.Reject(OpenIdConnectConstants.Errors.InvalidRequest, "Could not validate refresh_token.");
                return;
            }

            SetCorsHeader(context);

            var principal = new ClaimsPrincipal(context.Ticket.Principal);
            var ticket = CreateAuthenticationTicket(principal, context.Ticket.Properties, context.Options, context);

            context.Validate(ticket);
        }

        public override Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            AddCustomPropertiesTokenResponsePayload(context);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Set cross-origin HTTP request (Cors) header to allow requests from a different domains. 
        /// This Cors value is specific to an Application and set by when validating the client application (ValidateClientAuthenticationp).
        /// </summary>
        private static void SetCorsHeader(HandleTokenRequestContext context)
        {
            var allowedOrigin = context.HttpContext.Items["as:clientAllowedOrigin"] as string;
            if (allowedOrigin != null)
            {
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", new StringValues(allowedOrigin));
            }
        }
    }
}