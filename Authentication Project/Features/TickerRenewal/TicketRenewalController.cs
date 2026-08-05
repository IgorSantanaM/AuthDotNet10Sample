using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace StartcodeAuthentication.Features.TickerRenewal
{
    public class TicketRenewalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/TicketRenewal/getdata")]
        public async Task<IActionResult> GetData()
        {
            var result = await HttpContext.AuthenticateAsync();

            if(!result.Succeeded || result.Properties?.ExpiresUtc == null)
            {
                return Ok(new
                {
                    messsage = "Not Authenticated or no ExpiresUtc found",
                    remainingSeconds = 0
                });
            }

            var expiresUtc = result.Properties.ExpiresUtc.Value;
            var nowUtc = DateTimeOffset.UtcNow;

            var remaining = expiresUtc - nowUtc;
            var remainingSeconds = (int)Math.Floor(remaining.TotalSeconds);

            if (remainingSeconds < 0)
                remainingSeconds = 0;

            var msg = $"Authentication Ticket expires in {remainingSeconds} seconds";

            return Ok(new 
            {
                message = msg,
                remainingSeconds,
                issuedUtc = result.Properties.IssuedUtc,
                expiresUtc
            });

        }
    }
}
