using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StartcodeAuthentication.Features.Secure;

// URL: /Secure/Index
public class SecureController : Controller
{
    [Authorize(Policy = "finance")]
    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated == true)
            return View();
        else
            return Redirect("/user/AccessDenied");
    }
}
