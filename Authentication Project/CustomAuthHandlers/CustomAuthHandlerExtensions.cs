using Microsoft.AspNetCore.Authentication;

namespace StartcodeAuthentication.CustomAuthHandlers
{
    public static class CustomAuthHandlerExtensions
    {
        public static AuthenticationBuilder AddCustomAuth(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string? displayName,
            Action<CustomAuthHandlerOptions> configureOptions)
        {
            return builder.AddScheme<CustomAuthHandlerOptions, CustomAuthHandler>(
                authenticationScheme,
                displayName,
                configureOptions);
        }
    }
}
