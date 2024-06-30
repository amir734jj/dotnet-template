namespace Api.Attributes;

using Microsoft.AspNetCore.Authorization;
using Models.Enums;

public class RoleAuthorizedAttribute : AuthorizeAttribute
{
    public RoleAuthorizedAttribute(params RoleEnum[] roles)
    {
        Roles = string.Join(',', roles);
    }
}