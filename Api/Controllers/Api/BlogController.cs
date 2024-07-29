using System.Linq;
using System.Threading.Tasks;
using Api.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Models;

namespace Api.Controllers.Api;

[Authorize]
[Route("api/[controller]")]
public class BlogController(IBlogLogic blogLogic, UserManager<User> userManager, IUserLogic userLogic)
    : BasicCrudController<Unit>
{
    protected override IBasicLogic<Unit> BasicLogic()
    {
        return blogLogic;
    }

    protected override async Task<bool> AuthorizationGuard(int id)
    {
        var user = await userManager.FindByNameAsync(User.Identity!.Name!);

        var userDetails = await userLogic.Get(user!.Id);

        return user.Role.HasFlag(RoleEnum.Admin) || userDetails.Blogs.Any(x => x.Id == id);
    }
}