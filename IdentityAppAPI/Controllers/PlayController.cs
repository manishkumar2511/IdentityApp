using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAppAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        [HttpGet("get-players")]
        public IActionResult GetPlayers()
        {
            return Ok(new { message = "only authorized users can view players" });
        }
    }
}
