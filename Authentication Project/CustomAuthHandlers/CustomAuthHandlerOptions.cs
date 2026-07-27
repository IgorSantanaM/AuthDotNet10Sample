using Microsoft.AspNetCore.Authentication;

namespace StartcodeAuthentication.CustomAuthHandlers
{
    public class CustomAuthHandlerOptions : AuthenticationSchemeOptions
    {
        public static string DefaultAuthenticationScheme => "CustomAuthHandler";
        public string LoginPath { get; set; } = "/User/Login";
        public string CookieName { get; set; } = "AuthCookie";
        public string DefaultRedirectPath { get; set; } = "/AuthTest";
        public string AccessDeniedPath { get; set; } = "/User/AccessDenied";
    }
}