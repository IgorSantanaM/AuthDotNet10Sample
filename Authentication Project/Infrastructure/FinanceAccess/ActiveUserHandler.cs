using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
