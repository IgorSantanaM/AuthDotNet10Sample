using Microsoft.AspNetCore.Authorization;
using StartcodeAuthentication.Infrastructure.InvoiceAccess;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(o =>
        {
            //o.AddPolicy("AdminOnly", p =>
            //{
            //    p.RequireRole("Admin");
            //});

            o.AddPolicy("invoiceowner", policy => policy.AddRequirements(new InvoiceOwnershipRequirement()));

            o.AddPolicy("finance", p =>
            {
                p.Requirements.Add(new FinanceAccessRequirement());

            });
        });

        services.AddSingleton<IAuthorizationHandler, FinanceAccessHandler>();
        services.AddSingleton<IAuthorizationHandler, ManagementAccessHandler>();
        services.AddSingleton<IAuthorizationHandler, ActiveUserHandler>();
        services.AddSingleton<IAuthorizationHandler, InvoiceOwnershipHandler>();

        return services;
    }
}
