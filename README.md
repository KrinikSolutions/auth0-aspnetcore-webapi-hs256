# ASP.NET Core Web API using JWT (HS256)

This example demonstrates how you can secure an API generated with JSON Web Tokens which are issued by Auth0. This sample application was generated using the [ASP.NET Yeoman Generator](https://www.npmjs.com/package/generator-aspnet), and selecting a **Web API Application**.

## 1. Configure HS256 JSON Web Tokens

The first thing you need to do is to ensure that your Auth0 Application is configured to use HS256 to sign the JSON Web Token, which will sign the tokens using your Application's Client Secret.

Go to the Application in your Auth0 Dashboard and go to Settings > Advanced Settings > OAuth and ensure the **JsonWebToken Signature Algorithm** is set to **HS256**.

## 2. Specify Auth0 Settings

While in the Application settings area of your Auth0 Dashboard, copy the **Domain**, **Client ID** and **Client Secret** values for your application and add them to your `appsettings.json` file:

``` json
{
  "auth0": {
    "domain": "your domain",
    "clientId": "your client id",
    "clientSecret": "your client secret"
  }
}
```

## 3. Add the JWT middleware

Add the JWT middleware to your ASP.NET Core application, by adding the dependency to the `Microsoft.AspNetCore.Authentication.JwtBearer` package: 

```
{
  "dependencies": {
    "Microsoft.NETCore.App": {
      "version": "1.0.0-rc2-3002702",
      "type": "platform"
    },
    "Microsoft.AspNetCore.Mvc": "1.0.0-rc2-final",
    "Microsoft.AspNetCore.Server.IISIntegration": "1.0.0-rc2-final",
    "Microsoft.AspNetCore.Server.Kestrel": "1.0.0-rc2-final",
    "Microsoft.Extensions.Configuration.EnvironmentVariables": "1.0.0-rc2-final",
    "Microsoft.Extensions.Configuration.FileExtensions": "1.0.0-rc2-final",
    "Microsoft.Extensions.Configuration.Json": "1.0.0-rc2-final",
    "Microsoft.Extensions.Logging": "1.0.0-rc2-final",
    "Microsoft.Extensions.Logging.Console": "1.0.0-rc2-final",
    "Microsoft.Extensions.Logging.Debug": "1.0.0-rc2-final",
    "Microsoft.AspNetCore.Authentication.JwtBearer": "1.0.0-rc2-final"
  },
  // Rest omitted for brevity
}
```

Once this is done remember to do

```
dotnet restore
```

to restore all packages

## 4. Configure JWT middleware

Next you need to configure the JWT Middleware in the `Configure` method of your `Startup` class:

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    var keyAsBase64 = Configuration["auth0:clientSecret"].Replace('_', '/').Replace('-', '+');
    var keyAsBytes = Convert.FromBase64String(keyAsBase64);

    var options = new JwtBearerOptions
    {
        Audience = Configuration["auth0:clientId"],
        Authority = $"https://{Configuration["auth0:domain"]}/",
        TokenValidationParameters =
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyAsBytes)
        }
    };
    app.UseJwtBearerAuthentication(options);

    app.UseMvc();
}
```

## 5. Secure your Controller Actions

Next you can secure the controller actions for which a user needs to be authenticated by adding the `[Authorize]` attribute to the Action:

```
public class ValuesController : Controller
{
    [HttpGet]
    [Route("ping")]
    public string Ping()
    {
        return "All good. You don't need to be authenticated to call this.";
    }

    [Authorize]
    [HttpGet]
    [Route("secured/ping")]
    public string PingSecured()
    {
        return "All good. You only get this message if you are authenticated.";
    }
}
```

## 6. Test your application

You can test your application by obtaining an `id_token` from Auth0 and then passing that token in the `Authorization` header of a request as a Bearer token.

Here is a sample RAW request:

```
GET /secured/ping HTTP/1.1
Host: localhost:5000
Authorization: Bearer <your token>
```

Or using [RestSharp](http://restsharp.org/):

```
var client = new RestClient("http://localhost:5000/secured/ping");
var request = new RestRequest(Method.GET);
request.AddHeader("authorization", "Bearer <your token>");
IRestResponse response = client.Execute(request);
```

> TIP: An easy way to generate a token is by signing into your Auth0 account and then using the [/auth/ro endpoint](https://auth0.com/docs/api/authentication#!#post--oauth-ro) of the Authentication API. 