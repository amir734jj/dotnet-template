using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Attributes;
using Api.Configs;
using Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Models;
using Models.ViewModels.Identities;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Models.Enums;
using Models.ViewModels.Api;

namespace Api.Controllers.Api
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<User> _signManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserSetup _userSetup;
        private readonly IUserLogic _userLogic;

        public AccountController(JwtSettings jwtSettings, UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signManager, IUserSetup userSetup,
            IUserLogic userLogic)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _roleManager = roleManager;
            _signManager = signManager;
            _userSetup = userSetup;
            _userLogic = userLogic;
        }
        
        [AllowAnonymous]
        [HttpGet]
        [Route("IsAuthenticated")]
        [SwaggerOperation("IsAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(User.Identity.IsAuthenticated.ToString().ToLower());
        }
        
        [Authorize]
        [HttpGet]
        [Route("")]
        [SwaggerOperation("AccountInfo")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
                
            return Ok(user);
        }

        [DisallowAuthorized]
        [HttpPost]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            var role = !(await _userLogic.GetAll()).Any() ? RoleEnum.Admin : RoleEnum.User;

            var user = new User
            {
                Name = registerViewModel.Name,
                Email = registerViewModel.Email,
                UserName = registerViewModel.Username,
                Role = role
            };

            // Create user
            var identityResults = new List<IdentityResult>
            {
                await _userManager.CreateAsync(user, registerViewModel.Password)
            };
            
            // Create the role if not exist
            if (!await _roleManager.RoleExistsAsync(role.ToString()))
            {
                identityResults.Add(await _roleManager.CreateAsync(new IdentityRole<int>(role.ToString())));
            }
            
            // Register the user to the role
            identityResults.Add(await _userManager.AddToRoleAsync(user, role.ToString()));

            if (identityResults.All(x => x.Succeeded))
            {
                await _userSetup.Setup(user.Id);
                
                return Ok("Successfully registered!");
            }

            return BadRequest(new ErrorViewModel(new[] {"Failed to register!"}
                .Concat(identityResults.SelectMany(x => x.Errors.Select(y => y.Description))).ToArray()));
        }

        [DisallowAuthorized]
        [HttpPost]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            // Ensure the username and password is valid.
            var user = await _userManager.FindByNameAsync(loginViewModel.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                return BadRequest(new ErrorViewModel("The username or password is invalid."));
            }

            await _signManager.SignInAsync(user, true);

            // Set LastLoginTime
            await _userLogic.Update(user.Id, x => x.LastLoginTime = DateTimeOffset.Now);

            var token = ResolveToken(user);

            return Ok(token);
        }

        [Authorize]
        [HttpPost]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();

            return Ok("Logged-Out");
        }
        
        [Authorize]
        [HttpGet]
        [Route("Role/{userId}/{role}")]
        [SwaggerOperation("ChangeRole")]
        public async Task<IActionResult> ChangeRole([FromRoute]int userId, [FromRoute]RoleEnum role)
        {
            var result = new List<IdentityResult>();
            
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            // Remove existing role
            result.Add(await _userManager.RemoveFromRoleAsync(user, user.Role.ToString()));
            
            // Add new role to the user
            result.Add(await _userManager.AddToRoleAsync(user, role.ToString()));

            // Update user role identifier
            await _userLogic.Update(userId, x => x.Role = role);

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("Refresh")]
        [SwaggerOperation("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
                
            var token = ResolveToken(user);

            return Ok(token);
        }

        /// <summary>
        ///     Resolves a token given a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string ResolveToken(User user)
        {
            // Generate and issue a JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),    // use username as name
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var expires = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenDurationInMinutes);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}