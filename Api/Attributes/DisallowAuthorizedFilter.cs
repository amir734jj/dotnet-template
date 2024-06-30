namespace Api.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class DisallowAuthorizedFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            context.Result = new BadRequestObjectResult("Authenticated user is disallowed from accessing this route");
        }
    }
}