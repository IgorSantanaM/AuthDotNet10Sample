using Microsoft.AspNetCore.Mvc;

namespace StartcodeAuthentication.Features.Api
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return Challenge();
            }
            return Ok("API Response");
        }


    }
}
