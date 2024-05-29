using IdentityAppAPI.DTO.Account;
using IdentityAppAPI.Model;
using IdentityAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger; 
      

        public AccountController(
            JWTService jwtService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
        ILogger<AccountController> logger 
            
            )
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDTO>> RefreshUserToken()
        {
            var user=await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Name)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginData) 
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginData.userName);
                if (user == null)
                    return Unauthorized("Invalid UserName or Password");
                if (user.EmailConfirmed == false) return Unauthorized("Please Confirmed your Email");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginData.password, false);
                if (!result.Succeeded)
                    return Unauthorized("Invalid UserName or Password");

                var userDto = CreateApplicationUserDto(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return BadRequest("An error occurred during login. Please try again.");
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult>Register(RegisterDTO registerData)
        {
            if (await CheckEmailExistsAsync(registerData.email))
            {
                return BadRequest($"An Existing user using {registerData.email} plaese try again with another email");
            }
            var userToAdd = new User
            { 
                firstName = registerData.firstName.ToLower(),
                lastName = registerData.lastName.ToLower(),
                UserName=registerData.email.ToLower(),
                Email=registerData.email.ToLower(),
                EmailConfirmed=true

            };
            var  result=await _userManager.CreateAsync(userToAdd, registerData.password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok("Your account has been created");

        }

        private UserDTO CreateApplicationUserDto(User user)
        {
            return new UserDTO
            {
                firstName = user.firstName,
                lastName = user.lastName,
                JWT = _jwtService.CreateJWT(user)
            };
        }
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}
