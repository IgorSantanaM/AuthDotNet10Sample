using Microsoft.AspNetCore.Authentication;
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
        var username = loginCredentials.UserName;

        var myClaims = new List<Claim>
        {
            new Claim("sub", "1245"),
            new Claim("name", username),
            new Claim("email", "bob@tn-data.se"),
            new Claim("role", "developer"),
            new Claim("role", "admin")
        };

        var myIdentity = new ClaimsIdentity(claims: myClaims,
            authenticationType: "pwd",
            nameType: "name",
            roleType: "role");

        var myPrincipal = new ClaimsPrincipal(myIdentity);
        var parameters = new Dictionary<string, object>() {
            { "Param1", "Value1" },
            {"Param2", "Value2" },
            {"Param3", "Value3" }
        };

        var items = new Dictionary<string, string>()
        {
            {"Item1", "Value1" },
            {"Item2", "Value2" },
            {"Item3", "Value3" }
        };

        var properties = new AuthenticationProperties(items, parameters)
        {
            IsPersistent = true
        };
        await HttpContext.SignInAsync(myPrincipal, properties);

        if(Url.IsLocalUrl(loginCredentials.ReturnUrl))
        {
            return LocalRedirect(loginCredentials.ReturnUrl);
        }
        else
        {
            return LocalRedirect("/");
        }
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

