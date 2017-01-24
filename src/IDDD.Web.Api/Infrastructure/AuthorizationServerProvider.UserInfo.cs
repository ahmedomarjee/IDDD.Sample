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
using IDDD.Common.Cqs.Query;
using IDDD.Common.Cqs.Command;
using IDDD.App.Cqs;
using IDDD.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using IDDD.Domain.Membership;

namespace IDDD.Web.Api.Infrastructure
{
    public partial class AuthorizationServerProvider : OpenIdConnectServerProvider
    {

        public override Task ValidateUserinfoRequest(ValidateUserinfoRequestContext context)
        {
            return base.ValidateUserinfoRequest(context);
        }

        public override Task HandleUserinfoRequest(HandleUserinfoRequestContext context)
        {
            // Note: by default, OpenIdConnectServerHandler automatically handles userinfo requests and directly
            // writes the JSON response to the response stream. This sample uses a custom UserInfoController that
            // handles userinfo requests: context.SkipToNextMiddleware() is called to bypass the default
            // request processing executed by OpenIdConnectServerHandler.
            context.SkipToNextMiddleware();

            return Task.FromResult<object>(null);
        }

    }
}