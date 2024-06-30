namespace Api.Controllers.Api;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Attributes;
using Configs;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Enums;
using Models.Models;
using Models.ViewModels.Api;
using Models.ViewModels.Identities;
using Swashbuckle.AspNetCore.Annotations;

[Route("api/[controller]")]
public class AccountController(
    JwtSettings jwtSettings,
    UserManager<User> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    SignInManager<User> signManager,
    IUserSetup userSetup,
    IUserLogic userLogic)
    : Controller
{
    [AllowAnonymous]
    [HttpGet]
    [Route("IsAuthenticated")]
    [SwaggerOperation("IsAuthenticated")]
    public IActionResult IsAuthenticated()
    {
        return Ok(User.Identity!.IsAuthenticated.ToString().ToLower());
    }

    [DisallowAuthorized]
    [HttpPost]
    [Route("Register")]
    [SwaggerOperation("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
    {
        var role = !(await userLogic.GetAll()).Any() ? RoleEnum.Admin : RoleEnum.Tenant;

        var user = new User
        {
            Name = registerViewModel.Name,
            Email = registerViewModel.Email,
            UserName = registerViewModel.Username,
            Role = role,
        };

        // Create user
        var identityResults = new List<IdentityResult>
        {
            await userManager.CreateAsync(user, registerViewModel.Password),
        };

        // Create the role if not exist
        if (!await roleManager.RoleExistsAsync(role.ToString()))
        {
            identityResults.Add(await roleManager.CreateAsync(new IdentityRole<int>(role.ToString())));
        }

        // Register the user to the role
        identityResults.Add(await userManager.AddToRoleAsync(user, role.ToString()));

        if (identityResults.All(x => x.Succeeded))
        {
            await userSetup.Setup(user.Id);

            return Ok("Successfully registered!");
        }

        return BadRequest(new ErrorViewModel(new[] { "Failed to register!" }
            .Concat(identityResults.SelectMany(x => x.Errors.Select(y => y.Description))).ToArray()));
    }

    [DisallowAuthorized]
    [HttpPost]
    [Route("Login")]
    [SwaggerOperation("Login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
    {
        // Ensure the username and password is valid.
        var user = await userManager.FindByNameAsync(loginViewModel.Username);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginViewModel.Password))
        {
            return BadRequest(new ErrorViewModel("The username or password is invalid."));
        }

        await signManager.SignInAsync(user, true);

        // Set LastLoginTime
        await userLogic.Update(user.Id, x => x.LastLoginTime = DateTimeOffset.Now);

        var token = ResolveToken(user);

        return Ok(token);
    }

    [Authorize]
    [HttpPost]
    [Route("Logout")]
    [SwaggerOperation("Logout")]
    public async Task<IActionResult> Logout()
    {
        await signManager.SignOutAsync();

        return Ok("Logged-Out");
    }

    [Authorize]
    [HttpGet]
    [Route("Role/{userId:int}/{role}")]
    [SwaggerOperation("ChangeRole")]
    public async Task<IActionResult> ChangeRole([FromRoute] int userId, [FromRoute] RoleEnum role)
    {
        var result = new List<IdentityResult>();

        var user = await userManager.FindByIdAsync(userId.ToString());

        // Remove existing role
        result.Add(await userManager.RemoveFromRoleAsync(user!, user!.Role.ToString()));

        // Add new role to the user
        result.Add(await userManager.AddToRoleAsync(user, role.ToString()));

        // Update user role identifier
        await userLogic.Update(userId, x => x.Role = role);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    [Route("Refresh")]
    [SwaggerOperation("Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var user = await userManager.FindByNameAsync(User.Identity!.Name!);

        var token = ResolveToken(user);

        return Ok(token);
    }

    private string ResolveToken(User user)
    {
        // Generate and issue a JWT token
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!), // use username as name
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddMinutes(jwtSettings.AccessTokenDurationInMinutes);

        var token = new JwtSecurityToken(
            jwtSettings.Issuer,
            jwtSettings.Issuer,
            claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}