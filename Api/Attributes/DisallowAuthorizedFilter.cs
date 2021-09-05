using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Attributes
{
    public class DisallowAuthorizedAttribute : TypeFilterAttribute
    {
        public DisallowAuthorizedAttribute(): base(typeof(DisallowAuthorizedFilter)){}
    }

    public class DisallowAuthorizedFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new BadRequestObjectResult("Authenticated user is disallowed from accessing this route");
            }
        }
    }
}