using System.Threading.Tasks;
using Api.Abstracts;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Api.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class BlogController : BasicCrudController<Blog>
    {
        private readonly IBlogLogic _blogLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="blogLogic"></param>
        public BlogController(IBlogLogic blogLogic)
        {
            _blogLogic = blogLogic;
        }

        protected override async Task<IBasicLogic<Blog>> BasicLogic()
        {
            return _blogLogic;
        }
    }
}