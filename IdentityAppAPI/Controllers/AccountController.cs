using IdentityAppAPI.DTO.Account;
using IdentityAppAPI.Model;
using IdentityAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Text;
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
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountController(
            JWTService jwtService,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
        ILogger<AccountController> logger,
        EmailService emailService,
        IConfiguration config
            
            )
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailService = emailService;
            _configuration = config;
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
                return BadRequest($" {registerData.email} already exists plaese try with another email");
            }
            var userToAdd = new User
            { 
                firstName = registerData.firstName.ToLower(),
                lastName = registerData.lastName.ToLower(),
                UserName=registerData.email.ToLower(),
                Email=registerData.email.ToLower(),

            };
            var  result=await _userManager.CreateAsync(userToAdd, registerData.password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            try
            {
                if(await SendConfirmEmail(userToAdd))
                {
                  
                    return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created.please confirm your Email" }));
                }
                return BadRequest("Failed to send Email,please contact admin");

            }
            catch (Exception)
            {
                return BadRequest("Failed to send Email,please contact admin");
            }
            

        }
        [HttpPut("confirm-email")]
        public async Task<IActionResult>ConfirmEmail(ConfirmEmailDTO model)
        {
            var user=await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Email doesn't exists");
            if (user.EmailConfirmed == true) return BadRequest("Email already confirmed");
            try
            {
                var decodedTokenBytes=WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken=Encoding.UTF8.GetString(decodedTokenBytes);
                var result= await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email Confirmed",message="Your email is confirmed , you can login Now" }));
                }
                return BadRequest("Invalid Token!,Please try again");
            }
            catch (Exception)
            {
                return BadRequest("Invaild Token! ,Please try again ");
            }
        }
        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult>ResendEmailConfirmationLink(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Invalid Email");
            var user=await _userManager.FindByEmailAsync (email);
            if (user == null)
                return BadRequest("User not found");


            if (user.EmailConfirmed == true)
                return BadRequest("Email alredy confirmed , you can login now ");
            try
            {
                if(await  SendConfirmEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Confirmation Link sent", message = "Email Confirmation Link sent Again" }));
                }
                return BadRequest("Something went worng,please contact admin");

            }
            catch(Exception)
            {
                return BadRequest("something went worng , please contact admin");
            }

        }

        [HttpPost("forget-username-or-password/{email}")]
        public async Task<IActionResult>ForgetUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("User not found!");
            var user=await _userManager.FindByEmailAsync(email);

            if (user == null) return BadRequest ("User not found!");
            if (user.EmailConfirmed == false) return BadRequest("Email alredy confirmed , you can login now ");
            try
            {
                if(await SendForgetUsernameOrPassword(user))
                {
                    return Ok(new JsonResult(new {  message = "Forget usrname or Password  Link sent on Your registered email" }));

                }
                return BadRequest("Failed to send email please contact  admin");

            }
            catch (Exception)
            {
                return BadRequest("Somthing went worng contact admin");
            }

        }
        [HttpPut("reset-password")]

        public async Task<IActionResult>ResetPasword(ResetPasswordDTO model)
        {
            if (string.IsNullOrEmpty(model.Email)) return BadRequest("invalid Username");
            var user=await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("something went worng please contact admin");
            if (user.EmailConfirmed == false) return BadRequest("Email not verified yet ,please verify your email first");
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
                var result = await _userManager.ResetPasswordAsync(user, decodedToken,model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { message = "Your password has been  updated successfully"}));
                }
                return BadRequest("Invalid Token!,Please send reset link again");

            }
            catch (Exception)
            {
                return BadRequest("something went worng please contact admin");
            }


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
        private async Task<bool> SendConfirmEmail(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:clientUrl"]}/{_configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; background-color: #f9f9f9; border: 1px solid #ddd; border-radius: 10px;'>
                                <div style='background-color: #4CAF50; padding: 10px; border-radius: 10px 10px 0 0; color: white;'>
                                    <h2 style='margin: 0;'>Confirm your email</h2>
                                </div>
                                <div style='padding: 20px;'>
                                    <p><b>Hello, {user.firstName} {user.lastName}</b></p>
                                    <p>Please confirm your email by clicking on the link below:</p>
                                    <p><a href='{url}' style='color: #4CAF50;'>Confirm Email</a></p>
                                    <p>Thank you!</p>
                                    <br/>
                                    <b>{_configuration["Email:ApplicationName"]}</b>
                                </div>
                                <div style='padding: 10px; text-align: center;'>
                                    <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVNaXa6v7Bne-h3S3ENifuGvYmTDIAkws69g&s' alt='Footer Image' style='width: 100px;'/>
                                </div>
                            </div>";

            var emailSend = new SendEmailDTO(user.Email, "Confirm your Email", body);
            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgetUsernameOrPassword(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:clientUrl"]}/{_configuration["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";
            var body = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; background-color: white; border: 1px solid #ddd; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                            <div style='background-color: #ff0000; padding: 10px; border-radius: 10px 10px 0 0; color: white;'>
                                <h2 style='margin: 0; background-color: #ff0000;'>Reset Password</h2>
                            </div>
                            <div style='padding: 20px;'>
                                <b>Username: {user.Email}</b>
                                <p>In order to reset your password, please click on the following link</p>
                                <p><a href='{url}' style='color: #ff0000;'>Reset Password</a></p>
                                <p>Thank you!</p>
                                <br/>
                                <b>{_configuration["Email:ApplicationName"]}</b>
                            </div>
                            <div style='padding: 10px; text-align: center;'>
                                <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRVNaXa6v7Bne-h3S3ENifuGvYmTDIAkws69g&s' alt='Footer Image' style='width: 100px;'/>
                            </div>
                        </div>";
            var emailSend = new SendEmailDTO(user.Email, "Reset Your Password", body);
            return await _emailService.SendEmailAsync(emailSend);
        }

    }
}
