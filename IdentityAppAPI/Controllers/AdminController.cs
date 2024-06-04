using IdentityAppAPI.DTO.Admin;
using IdentityAppAPI.Model;
using IdentityAppAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAppAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ContextSeedService> _logger;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ContextSeedService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<IEnumerable<MemberViewDTO>>> GetMembers()
        {
            var users = await _userManager.Users
                .Where(x => x.UserName != SeedDataBase.AdminUserName)
                .ToListAsync();

            var memberList = new List<MemberViewDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isLocked = await _userManager.IsLockedOutAsync(user);

                memberList.Add(new MemberViewDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    DateCreated = user.dateCreated,
                    IsLocked = isLocked,
                    Roles = roles.ToList()
                });
            }

            return Ok(memberList);
        }
        [HttpPut("lock-member/id")]
        public async Task<IActionResult>LockMember(string id)
        {
            var user= await  _userManager.FindByIdAsync(id);
            if (user == null) return BadRequest("User not found ");
            if (IsAdminUserId(user.Id)) return BadRequest(SeedDataBase.SuperAdminChangeNotAllowed);

            await _userManager.SetLockoutEndDateAsync(user,DateTime.UtcNow.AddDays(1));
            return NoContent();
        }
        private  bool IsAdminUserId(string userId)
        {
            return _userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SeedDataBase.AdminUserName);
        }
    }
}
