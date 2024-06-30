namespace Api.Controllers.Api;

using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Models.ViewModels.Api;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize]
[ApiController]
[Route("Api/[controller]")]
public class ProfileController(UserManager<User> userManager, IProfileLogic profileLogic)
    : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.FindByNameAsync(User.Identity!.Name!);

        var profile = await profileLogic.Get(user);

        return Ok(profile);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Update([FromBody] ProfileViewModel profileViewModel)
    {
        var user = await userManager.FindByNameAsync(User.Identity!.Name!);

        await profileLogic.Update(user, profileViewModel);

        return RedirectToAction("Index");
    }
}