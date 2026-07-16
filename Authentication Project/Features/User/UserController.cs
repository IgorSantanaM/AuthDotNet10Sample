using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StartcodeAuthentication.Features.User;

public class UserController : Controller
{
    [HttpGet]
    public IActionResult Login(string ReturnUrl)
    {
        return View(new LoginModel() { ReturnUrl = ReturnUrl });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginCredentials)
    {
        return Redirect(loginCredentials.ReturnUrl ?? "/");
    }


    [HttpPost]
    public async Task Logout()
    {
    }


    public IActionResult LoggedOut()
    {
        return View();
    }


    public IActionResult AccessDenied()
    {
        return View();
    }


    public IActionResult Info()
    { 
        // 1. Get the user (ClaimsPrincipal) from the HttpContext
        ClaimsPrincipal user = User;
        // 2. Get the Primary Identity of the user (there might be more than one)
        ClaimsIdentity identity = user.Identity as ClaimsIdentity;
        // 3. Get IsAuthenticated
        bool isAuthenticated = identity.IsAuthenticated;

        // 4. Get AuthenticationType
        string authenticationType = identity.AuthenticationType;

        // 5. Get the claims from the user
        IEnumerable<Claim> claims = isAuthenticated ? user.Claims : null;

        // 6. Get the Name claim
        string name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        // 7. Check if user has developer or admin role
       var isDeveloper = user.IsInRole("Developer");
        var isAdmin = user.IsInRole("Admin");   

        var model = new UserInfoModel()
        {
            IsAuthenticated = isAuthenticated,
            AuthenticationType = authenticationType,
            Claims = claims?.ToList(),
            Name = name,
            IsDeveloper = isDeveloper,
            IsAdmin = isAdmin,
            DefaultNameClaimType = identity.NameClaimType,
            DefaultRoleClaimType = identity.RoleClaimType
        };

        return View(model);
    }
}

