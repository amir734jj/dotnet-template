using System.Linq;
using System.Threading.Tasks;
using Api.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Models.Models;

namespace Api.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : BasicCrudController<Blog>
    {
        private readonly IBlogLogic _blogLogic;
        private readonly UserManager<User> _userManager;
        private readonly IUserLogic _userLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="blogLogic"></param>
        /// <param name="userManager"></param>
        /// <param name="userLogic"></param>
        public BlogController(IBlogLogic blogLogic, UserManager<User> userManager, IUserLogic userLogic)
        {
            _blogLogic = blogLogic;
            _userManager = userManager;
            _userLogic = userLogic;
        }

        protected override async Task<IBasicLogic<Blog>> BasicLogic()
        {
            return _blogLogic;
        }

        protected override async Task<bool> AuthorizationGuard(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userDetails = await _userLogic.Get(user.Id);

            return user.Role.HasFlag(RoleEnum.Admin) || userDetails.Blogs.Any(x => x.Id == id);
        }
    }
}