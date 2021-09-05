using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Api
{
    [RoleAuthorized(RoleEnum.Admin)]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserLogic _userLogic;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userLogic"></param>
        /// <param name="userManager"></param>
        public UserController(IUserLogic userLogic, UserManager<User> userManager)
        {
            _userLogic = userLogic;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userLogic.GetAll();

            return Ok(users);
        }
    }
}