using Microsoft.AspNetCore.Authorization;
using Models.Enums;

namespace Api.Attributes
{
    public class RoleAuthorizedAttribute : AuthorizeAttribute
    {
        public RoleAuthorizedAttribute(params RoleEnum[] roles)
        {
            Roles = string.Join(',', roles);
        }
    }
}