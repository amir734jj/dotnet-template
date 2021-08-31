using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("health")]
    public class HealthController : Controller
    {
        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return Ok(new
            {
                RemoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                LocalIpAddress = HttpContext.Connection.LocalIpAddress.ToString(),
            });
        }
    }
}