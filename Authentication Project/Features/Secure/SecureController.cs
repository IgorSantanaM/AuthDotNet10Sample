using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartcodeAuthentication.Infrastructure;
using StartcodeAuthentication.Infrastructure.Filters;

namespace StartcodeAuthentication.Features.Secure;

// URL: /Secure/Index
public class SecureController : Controller
{
    [TypeFilter(typeof(AddClaimsFilter))]
    //[Authorize(Policy = "finance")]
    [RequireFinancePolicy]
    public IActionResult Index()
    {
        if (User?.Identity?.IsAuthenticated == true)
            return View();
        else
            return Redirect("/user/AccessDenied");
    }
}

public class RequireFinancePolicyAttribute : AuthorizeAttribute
{
    public RequireFinancePolicyAttribute() : base(Policies.FinanceAccessPolicy)
    {

    }
}
