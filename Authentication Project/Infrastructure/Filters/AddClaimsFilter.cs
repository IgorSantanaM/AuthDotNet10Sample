using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mono.TextTemplating;
using System.Security.Claims;

namespace StartcodeAuthentication.Infrastructure.Filters
{
    public class AddClaimsFilter(IConfiguration configuration) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("Called");

            var hash = new HashSet<string>();

            hash = configuration.GetSection("BlockedIps")
                .Get<string[]>() ?.ToHashSet(StringComparer.OrdinalIgnoreCase);

            var identity = context.HttpContext.User?.Identity as ClaimsIdentity;

            if (hash.Contains(context.HttpContext.Connection.RemoteIpAddress.ToString()))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.HttpContext.Response.WriteAsync("Access Denied.");
                return;
            }
            if(identity != null && identity.IsAuthenticated)
            {
                identity.AddClaim(new Claim(
                    type: "request_time",
                    value: DateTimeOffset.UtcNow.ToString(),
                    valueType: ClaimValueTypes.String,
                    issuer: "AddClaimFilter"));

                identity.AddClaim(new Claim(
                    type: "dynamic_role",
                    value: "temp_admin",
                    valueType: ClaimValueTypes.String,
                    issuer: "AddClaimFilter"));


                var path = context.HttpContext.Request.Path;

                if(path.StartsWithSegments("/Secure/Support"))
                {
                    var hasEmail = identity.HasClaim(c => c.Type == "email");

                    if(!hasEmail)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
        }
    }
}


