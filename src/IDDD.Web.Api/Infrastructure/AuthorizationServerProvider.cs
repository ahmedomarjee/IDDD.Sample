using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.Extensions.Configuration;
using AuthenticationProperties = Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties;
using IDDD.App.Cqs.QueryResult.Users;
using IDDD.App.Cqs.Commands.RefreshTokens;
using IDDD.Common.Cqs.Query;
using IDDD.App;
using IDDD.Common;

namespace IDDD.Web.Api.Infrastructure
{
    public partial class AuthorizationServerProvider : OpenIdConnectServerProvider
    {





        /// <summary>
        /// Creates a valid authentication token used to create the access_token.
        /// </summary>
        private static AuthenticationTicket CreateAuthenticationTicket(LoginResult result, HandleTokenRequestContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.Name, result.UserName, OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            identity.AddClaim(ClaimTypes.NameIdentifier, result.UserId.ToString(), OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
            var properties = new AuthenticationProperties();
            var principal = new ClaimsPrincipal(new[] { identity });

            return CreateAuthenticationTicket(principal, properties, context.Options, context);
        }

        private static AuthenticationTicket CreateAuthenticationTicket(
            ClaimsPrincipal principal,
            AuthenticationProperties authenticationProperties,
            OpenIdConnectServerOptions options, BaseContext context)
        {
            var configuration = Configuration(context);
            var ticket = new AuthenticationTicket(principal,
                authenticationProperties,
                options.AuthenticationScheme);
            ticket.SetResources(configuration.ApiHostName());
            ticket.SetScopes(
                OpenIdConnectConstants.Scopes.OfflineAccess);
            return ticket;
        }

        private static void AddCustomPropertiesTokenResponsePayload(ApplyTokenResponseContext context)
        {
            foreach (var property in context.HttpContext.Items.Where(item => item.Key.ToString().StartsWith("as:")))
            {
                context.Response.Add(property.Key as string, new JValue(property.Value));
            }
        }

        public override async Task SerializeRefreshToken(SerializeRefreshTokenContext context)
        {
            await StoreRefreshToken(context);
        }

        private async Task StoreRefreshToken(SerializeRefreshTokenContext context)
        {
            var principal = context.Ticket.Principal;
            var properties = context.Ticket.Properties;

            var command = new CreateRefreshTokenCommand(
                context.Ticket.GetTicketId(),
                context.Request.ClientId,
                principal.GetClaim(ClaimTypes.NameIdentifier),
                principal.GetClaim(ClaimTypes.Name),
                context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                properties.IssuedUtc.GetValueOrDefault().DateTime,
                properties.ExpiresUtc.GetValueOrDefault().DateTime);

            var result = await ExecuteCommand<CreateRefreshTokenCommand,Result>(context, command);
            if (result.Succeeded == false)
            {
                throw new InvalidOperationException("Could not store the refreshtoken");
            }
        }

        private static async Task<TResult> ExecuteQuery<TResult>(BaseContext context, IQuery<TResult> query)
        {
            var queryProcessor = context.HttpContext.RequestServices.GetQueryProcessor();
            return await queryProcessor.ProcessAsync<TResult>(query);
        }

        private static async Task<TResult> ExecuteCommand<TCommand,TResult>(BaseContext context, TCommand command)
        {
            var commandDispatcher = context.HttpContext.RequestServices.GetCommandDispatcher();
            return await commandDispatcher.DispatchAsync<TCommand, TResult>(command);
        }

        private static IConfiguration Configuration(BaseContext context)
        {
            return context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        }

    }
}