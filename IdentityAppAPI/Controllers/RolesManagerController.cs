using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesManagerController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {

            return Ok("Public method ");
        }


        #region Roles

        [HttpGet("role-admin")]
        [Authorize(Roles ="Admin")]
        public IActionResult AdminRole()
        {

            return Ok("Admin Role ");
        }

        [HttpGet("role-manager")]
        [Authorize(Roles = "Manager")]
        public IActionResult ManagerRole()
        {

            return Ok("Manager Role ");
        }

        [HttpGet("role-player")]
        [Authorize(Roles = "Player")]
        public IActionResult PlayerRole()
        {

            return Ok("Player Role ");
        }

        [HttpGet("admin-or-manager-role")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult AdminOrMAnagerRole()
        {

            return Ok("Admin or Manager Role ");
        }

        [HttpGet("admin-or-player-role")]
        [Authorize(Roles = "Admin,Player")]
        public IActionResult AdminOrPlayerRole()
        {

            return Ok("Admin or Player Role ");
        }
        #endregion

        #region Policy
        [HttpGet("admin-policy")]
        [Authorize(policy: "AdminPolicy")]
        public IActionResult AdminPolicy()
        {

            return Ok("Admin Policy ");
        }

        [HttpGet("manager-policy")]
        [Authorize(policy: "ManagerPolicy")]
        public IActionResult ManagerPolicy()
        {

            return Ok("Manager Policy ");
        }

        [HttpGet("player-policy")]
        [Authorize(policy: "PlayerPolicy")]
        public IActionResult PlayerPolicy()
        {

            return Ok("Player Policy ");
        }

        [HttpGet("admin-or-manager-policy")]
        [Authorize(policy: "AdminOrManagerPolicy")]
        public IActionResult AdminOrManagerPolicy()
        {

            return Ok("Admin Or Manager Policy ");
        }

        [HttpGet("admin-and-manager-policy")]
        [Authorize(policy: "AdminAndManagerPolicy")]
        public IActionResult AdminAndManagerPolicy()
        {

            return Ok("Admin and Manager Policy ");
        }

        [HttpGet("all-role-policy")]
        [Authorize(policy: "AllRolePolicy")]
        public IActionResult AllRolePolicy()
        {

            return Ok("All Role Policy");
        }
        #endregion
        #region Claim Policy
        [HttpGet("admin-email-policy")]
        [Authorize(policy: "AdminEmailPolicy")]
        public IActionResult AdminEmailPolicy()
        {

            return Ok("Admin Email Policy ");
        }

        [HttpGet("manager-email-policy")]
        [Authorize(policy: "ManagerEmailPolicy")]
        public IActionResult ManagerEmailPolicy()
        {

            return Ok("Manager Email Policy  ");
        }

        #endregion
    }
}
