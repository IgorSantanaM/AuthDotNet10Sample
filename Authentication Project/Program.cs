using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Support features folder structure
builder.Services.Configure<RazorViewEngineOptions>(rvo =>
{
    rvo.ViewLocationFormats.Add("~/Features/{1}/{0}.cshtml");
    rvo.ViewLocationFormats.Add("~/Views/Shared/{0}.cshtml");
});

builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = "cookie";
}).AddCookie("cookie", o =>
{
    o.LoginPath = "/User/Login";
    o.AccessDeniedPath = "/User/AccessDenied";
    //o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = (context) =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            // For API requests, return 401 Unauthorized instead of redirecting to the login page
            context.Response.StatusCode = 401;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };

});

builder.Services.AddAuthorizationServices();
//.AddCustomAuth(authenticationScheme: "handler1",
//            displayName: "myAuth",
//            configureOptions: o =>
//            {
//                o.LoginPath = "/User/Login";
//            });
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Use(async (context, next) =>
{
    var myClaims = new List<Claim>()
 {
     new("sub", "12345"),
     new(@"name", "John Doe"),
     new("email", "john@doe.com"),
     new("role", "Admin")
 };

    var myIdentity = new ClaimsIdentity(claims: myClaims,
        authenticationType: "CustomAuth",
        nameType: "name",
        roleType: "role");

    var myPrincipal = new ClaimsPrincipal(myIdentity);

    context.User = myPrincipal;
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

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

            o.AddPolicy("finance", p =>
            {
                p.Requirements.Add(new FinanceAccessRequirement());

            });
        });

        services.AddSingleton<IAuthorizationHandler, FinanceAccessHandler>();
        services.AddSingleton<IAuthorizationHandler, ManagementAccessHandler>();
        services.AddSingleton<IAuthorizationHandler, ActiveUserHandler>();

        return services;
    }
}

public class FinanceAccessRequirement : IAuthorizationRequirement
{
   
}

public class FinanceAccessHandler(ILogger<FinanceAccessHandler> logger) : AuthorizationHandler<FinanceAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FinanceAccessRequirement requirement)
    {
        logger.LogInformation("FinanceAccessHandler: Checking if user has finance access.");
        var user = context.User;

        var titleOk = user.HasClaim(c => c.Type == "JobTitle" && c.Value == "finance");

        var countryOk = user.HasClaim(c => c.Type == "Country" && (c.Value == "USA") || (c.Value == "UK"));

        var rolesOk = user.IsInRole("finance");

        if(titleOk && countryOk && rolesOk)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}

public class ManagementAccessHandler(ILogger<ManagementAccessHandler> logger) : AuthorizationHandler<FinanceAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FinanceAccessRequirement requirement)
    {
        logger.LogInformation("ManagementAccessHandler: Checking if user is in management role.");
        if (context.User.IsInRole("management"))
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}

public class ActiveUserHandler : AuthorizationHandler<FinanceAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FinanceAccessRequirement requirement)
    {
        var user = context.User;
       bool isActive = IsUserActive(user);

        if(isActive)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }

    private bool IsUserActive(ClaimsPrincipal user)
    {
        return true;
    }
}
