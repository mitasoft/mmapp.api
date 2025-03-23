using Microsoft.AspNetCore.Mvc;

namespace MMApp.Api.Controllers.vx
{
    [Route("api/vx/health")]
    [ApiController]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
