using Microsoft.AspNetCore.Authorization;

public class FinanceAccessHandler(ILogger<FinanceAccessHandler> logger) : AuthorizationHandler<FinanceAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FinanceAccessRequirement requirement)
    {
        logger.LogInformation("FinanceAccessHandler: Checking if user has finance access.");
        var user = context.User;

        var titleOk = user.HasClaim(c => c.Type == "JobTitle" && c.Value == "finance");

        var countryOk = user.HasClaim(c => c.Type == "Country" && (c.Value == "USA") || (c.Value == "UK"));

        var rolesOk = user.IsInRole("finance");

        if(titleOk && countryOk && rolesOk)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}
