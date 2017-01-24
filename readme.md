# Token-authentication/authorization in .Net Core with OpenIdConnect and MongoDB #

### Summary ###
Sample shows how you can use **AspNet.Security.OpenIdConnect.Server** framework for integrating token-based authentication/authorization in AspNet Core.  

Project is developed as part of custom authorization functionality for securing web clients by incorporating OAuth password-credentials flow.

MongoDB is used as persistance store with custom implementation of the AspNetCore.Identity interfaces on top of which is designed custom authorization mechanism with OpenIdConnect.

### Prerequisites ###
 - In order to run this sample should install MongoDB instance and apply necessary changes in appsettings.json
 - Testing the endpoints can be done with Postman - postman-readme or IDDD.postman_collection.json.
 
### Solution ###
Solution | Author(s)
---------|----------
IDDD.Sample | Ilker Karimanov

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 2016 | Initial release

### Future improvements

- Extend scenario with implicit grant flow (probably in another solution).

*PS: Any suggestions will be greatly appreciated*

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Q/A #
**Q**: Why using OpenIdConnect.Server? 

**A**: It is very well-defined fully dedicated to protocol-first approach framework, which is giving you a good ground to step-on and build custom auth flows.

*Note*: Identity Server 4 is another full-blown alternative.

**Q**: What is the purpose of IDDD.Domain.Clients?

**Short A**: Security.

**Long A**: It is expected if user as successfully authenticated by username/password at least to be associated with some security domain, something which identifies the client/origin. Client interface store information for the origin which is critical if we want to secure against unknow cross-origin requests.
Client interface also support types like Javascript(or secure clients...) as you might want to differentiate between supported auth scenerios and client types - passsword, implicit etc.


**Q**: What is the IDDD.Domain.RefreshTokens?

**A**: Implementation of refreshing auth tokens without starting authentication again with username and password.

# Code Samples #

## Authentication Configuration ##

Code snippet:
```C#
...
app.UseWhen(IsApi, ApiAuthentication(configuration));
app.UseOpenIdConnectServer(ServerOptions);
return app;   
```

## Api Authentication ##

Code snippet:
```C#
var options = new OAuthValidationOptions
{
     AutomaticAuthenticate = true,
     AutomaticChallenge = true
};

options.Audiences.Add(configuration.ApiHostName());

return branch => branch.UseOAuthValidation(options);       
```

## OpenIdConnect Configuration ##

Code snippet(For testing only):
```C#
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
```






