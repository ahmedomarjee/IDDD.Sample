using System;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Server;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.Extensions.DependencyInjection;
using IDDD.App.Cqs.Commands.RefreshTokens;
using IDDD.App.Cqs.Queries.Clients;
using Microsoft.Extensions.Primitives;

namespace IDDD.Web.Api.Infrastructure
{
    public partial class AuthorizationServerProvider : OpenIdConnectServerProvider
    {

        public override async Task ValidateRevocationRequest(ValidateRevocationRequestContext context)
        {

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

            // When token_type_hint is specified, reject the request if it doesn't correspond to a revocable token.
            if (!string.IsNullOrEmpty(context.TokenTypeHint) &&
                !string.Equals(context.TokenTypeHint, OpenIdConnectConstants.TokenTypeHints.RefreshToken))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedTokenType,
                    description: "Only refresh tokens can be revoked. When specifying a token_type_hint " +
                                 "parameter, its value must be equal to 'refresh_token'.");

                return;
            }
            context.Validate();
        }

        public override async Task HandleRevocationRequest(HandleRevocationRequestContext context)
        {
            // If the received token is not a refresh token,
            // return an error to indicate that the token cannot be revoked.
            if (!context.Ticket.IsRefreshToken())
            {
                
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedTokenType,
                    description: "Only refresh tokens can be revoked.");

                return;
            }
            // Extract the token identifier from the authentication ticket.
            var identifier = context.Ticket.GetTicketId();

            var cmd = new DeleteRefreshTokenByTokenCommand(identifier);
            var result = await ExecuteCommand<DeleteRefreshTokenByTokenCommand, Core.Result>(context, cmd);
            if (result.Succeeded)
            {
                context.Revoked = true;
                SetCorsHeader(context);
            }
            else
            {
                context.Reject(OpenIdConnectConstants.Errors.InvalidRequest, "Could not revoke refresh_token.");
                return;
            }
        }

        /// <summary>
        /// Set cross-origin HTTP request (Cors) header to allow requests from a different domains. 
        /// This Cors value is specific to an Application and set by when validating the client application (ValidateClientAuthenticationp).
        /// </summary>
        private static void SetCorsHeader(HandleRevocationRequestContext context)
        {
            var allowedOrigin = context.HttpContext.Items["as:clientAllowedOrigin"] as string;
            if (allowedOrigin != null)
            {
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", new StringValues(allowedOrigin));
            }
        }

    }
}