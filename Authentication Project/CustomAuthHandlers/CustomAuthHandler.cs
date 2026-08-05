using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Data.Common;
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
            var path = Request?.Path ?? "[Unknown]";
            WriteToLog($"HandleAuthenticateAsync: Called for {path}");

            var authCookie = Request.Cookies[Options.CookieName];
            if (string.IsNullOrEmpty(authCookie))
            {
                WriteToLog($"No cookie {Options.CookieName} found - user not authenticated");
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            else
            {
                try
                {
                    byte[] serializedTicket = Convert.FromBase64String(authCookie);

                    var provider = DataProtectionProvider.Create("MyApp");
                    var protector = provider.CreateProtector("AuthTicket");
                    var unprotectedBytes = protector.Unprotect(serializedTicket);

                    AuthenticationTicket ticket = TicketSerializer.Default.Deserialize(unprotectedBytes);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid authentication cookie"));
                }
            }
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            Response.Cookies.Append(key: Options.CookieName, value: user.Identity?.Name ?? "UnknownUser", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

            var ticket = new AuthenticationTicket(user, properties, Scheme.Name);

            byte[] serializedTicket = TicketSerializer.Default.Serialize(ticket);

            var provider = DataProtectionProvider.Create("MyApp");
            var protector = provider.CreateProtector("AuthTicket");
            var protectedBytes = protector.Protect(serializedTicket);

            var cookieValue = Convert.ToBase64String(protectedBytes);

            Response.Cookies.Append(Options.CookieName, cookieValue, new CookieOptions
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
