using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace StartcodeAuthentication.CustomAuthHandlers
{
    public class CustomAuthHandler : SignInAuthenticationHandler<CustomAuthHandlerOptions>
    {
        public CustomAuthHandler(IOptionsMonitor<CustomAuthHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            WriteToLog("CustomAuthHandler created");
        }

        private void WriteToLog(string message)
        {
            var scheme = Scheme?.Name ?? "[Default]";

            var msg = $"### [{DateTime.Now:HH:mm:ss} - {scheme} - {message}";
            Console.WriteLine(msg);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var path = Request.Path.Value ?? "[Unknown]";
            WriteToLog($"HandleAuthenticateAsync: Called for {path}");

            var username = Request.Cookies[Options.CookieName];

            if (string.IsNullOrEmpty(username))
            {
                WriteToLog("HandleAuthenticateAsync: No cookie found, returning NoResult");
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            List<Claim> claims = new() { new Claim(ClaimTypes.Name, username) };


            return Task.FromResult(AuthenticateResult.NoResult());

        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
           Response.Cookies.Append(key: Options.CookieName, value: user.Identity?.Name ?? "UnknownUser", new CookieOptions
           {
               HttpOnly = true,
               SameSite = SameSiteMode.Lax
           });

            var redirectUrl = Options.DefaultRedirectPath;
            Response.Redirect(redirectUrl);

            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(Options.CookieName);
            var redirectUrl = Options.DefaultRedirectPath;
            Response.Redirect(redirectUrl);
            return Task.CompletedTask;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Context.ChallengeAsync();
            var redirectUrl = Options.LoginPath;
            Response.Redirect(redirectUrl);
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var redirectUrl = Options.AccessDeniedPath;
            Response.Redirect(redirectUrl);
            return Task.CompletedTask;
        }
    }
}
