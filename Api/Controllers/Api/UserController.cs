using Api.Abstracts;
using Api.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Models;

namespace Api.Controllers.Api;

[RoleAuthorized(RoleEnum.Admin)]
[Route("api/[controller]")]
public class UserController(IUserLogic userLogic) : BasicCrudController<User>
{
    protected override IBasicLogic<User> BasicLogic()
    {
        return userLogic;
    }
}