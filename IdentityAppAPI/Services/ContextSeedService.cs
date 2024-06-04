using IdentityAppAPI.Data;
using IdentityAppAPI.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityAppAPI.Services
{
    public class ContextSeedService
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ContextSeedService> _logger;

        public ContextSeedService(
            Context context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ContextSeedService> logger
        )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeContext()
        {
            try
            {
                _logger.LogInformation("Applying pending migrations...");
                if ((await _context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully.");
                }

                if (!await _roleManager.Roles.AnyAsync())
                {
                    _logger.LogInformation("Seeding roles...");
                    await _roleManager.CreateAsync(new IdentityRole(SeedDataBase.AdminRole));
                    await _roleManager.CreateAsync(new IdentityRole(SeedDataBase.ManagerRole));
                    await _roleManager.CreateAsync(new IdentityRole(SeedDataBase.PlayerRole));
                    _logger.LogInformation("Roles seeded successfully.");
                }

                if (!await _userManager.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding users...");
                    var admin = new User
                    {
                        firstName = "manish",
                        lastName = "kumar",
                        UserName = SeedDataBase.AdminUserName,
                        Email = SeedDataBase.AdminUserName,
                        EmailConfirmed = true,
                    };
                    var adminResult = await _userManager.CreateAsync(admin, "admin@123");
                    if (adminResult.Succeeded)
                    {
                        await _userManager.AddToRolesAsync(admin, new[] { SeedDataBase.AdminRole, SeedDataBase.ManagerRole, SeedDataBase.PlayerRole });
                        await _userManager.AddClaimsAsync(admin, new Claim[] {
                            new Claim(ClaimTypes.Email, admin.Email),
                            new Claim(ClaimTypes.GivenName, admin.firstName)
                        });
                        _logger.LogInformation("Admin user created successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", adminResult.Errors.Select(e => e.Description)));
                    }

                    var manager = new User
                    {
                        firstName = "sehaj",
                        lastName = "pal",
                        UserName = "manager@gmail.com",
                        Email = "manager@gmail.com",
                        EmailConfirmed = true,
                    };
                    var managerResult = await _userManager.CreateAsync(manager, "manager@123");
                    if (managerResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(manager, SeedDataBase.ManagerRole);
                        await _userManager.AddClaimsAsync(manager, new Claim[] {
                            new Claim(ClaimTypes.Email, manager.Email),
                            new Claim(ClaimTypes.GivenName, manager.firstName)
                        });
                        _logger.LogInformation("Manager user created successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create manager user: {Errors}", string.Join(", ", managerResult.Errors.Select(e => e.Description)));
                    }

                    var player = new User
                    {
                        firstName = "sehaj",
                        lastName = "pal",
                        UserName = "player@gmail.com",
                        Email = "player@gmail.com",
                        EmailConfirmed = true,
                    };
                    var playerResult = await _userManager.CreateAsync(player, "player@123");
                    if (playerResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(player, SeedDataBase.PlayerRole);
                        await _userManager.AddClaimsAsync(player, new Claim[] {
                            new Claim(ClaimTypes.Email, player.Email),
                            new Claim(ClaimTypes.GivenName, player.firstName)
                        });
                        _logger.LogInformation("Player user created successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create player user: {Errors}", string.Join(", ", playerResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("Users already exist, skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the seeding process.");
                throw;
            }
        }
    }
}