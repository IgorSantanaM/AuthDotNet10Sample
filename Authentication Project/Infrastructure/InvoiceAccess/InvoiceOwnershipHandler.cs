using Microsoft.AspNetCore.Authorization;

namespace StartcodeAuthentication.Infrastructure.InvoiceAccess
{
    public class InvoiceOwnershipHandler : AuthorizationHandler<InvoiceOwnershipRequirement>
    {
        //IAuthorizationService authorizationService;

        public InvoiceOwnershipHandler()
        {
            
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InvoiceOwnershipRequirement requirement)
        {
            // To cal the authorization service whenever it should be called.
            //authorizationService.AuthorizeAsync(context.User, resource: null, policyName: "invoiceownership");

            Console.WriteLine("Called");
            var userId = context.User.FindFirst("sub")?.Value;
            if(userId == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if (int.TryParse(userId, out int id))
            {
                // Check if the user is the owner of the invoice
                var invoiceIdClaim = context.User.FindFirst("invoiceId")?.Value;
                if (invoiceIdClaim != null && int.TryParse(invoiceIdClaim, out int invoiceId))
                {
                    if (id == invoiceId)
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
