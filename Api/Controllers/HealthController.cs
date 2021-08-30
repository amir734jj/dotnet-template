using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return Ok(new
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress
            });
        }
    }
}