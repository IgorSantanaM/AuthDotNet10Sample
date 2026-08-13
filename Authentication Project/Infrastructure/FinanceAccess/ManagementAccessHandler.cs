using Microsoft.AspNetCore.Authorization;

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
