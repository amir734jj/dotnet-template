using System.Threading.Tasks;
using Api.Abstracts;
using Api.Attributes;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Models;

namespace Api.Controllers.Api
{
    [RoleAuthorized(RoleEnum.Admin)]
    [Route("api/[controller]")]
    public class UserController : BasicCrudController<User>
    {
        private readonly IUserLogic _userLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userLogic"></param>
        public UserController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        protected override async Task<IBasicLogic<User>> BasicLogic()
        {
            return _userLogic;
        }
    }
}