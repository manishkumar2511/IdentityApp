using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityAppAPI.Services;
using IdentityAppAPI.Model;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityAppAPI.DTO.SSO;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SSOController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly UserManager<User> _userManager;

        public SSOController(JWTService jwtService, UserManager<User> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> SSOLogin([FromBody] TokenDTO tokenDto)
        {
            var claimsPrincipal = _jwtService.ValidateToken(tokenDto.Token);
            if (claimsPrincipal == null) return Unauthorized();

            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            // Create identity and sign in the user 
            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.firstName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

           // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Ok(new { redirectUrl = "http://localhost:52036/" });

        }
    }
}
