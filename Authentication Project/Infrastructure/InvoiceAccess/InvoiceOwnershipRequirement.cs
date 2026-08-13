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

public class InvoiceOwnershipRequirement : IAuthorizationRequirement
{
}
