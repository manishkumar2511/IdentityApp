using Auth0.AspNetCore.Authentication;
using IdentityAppAPI.Data;
using IdentityAppAPI.Model;
using IdentityAppAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Scoped Services
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ContextSeedService>();

// Identity Configuration
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<Context>()
.AddSignInManager<SignInManager<User>>()
.AddUserManager<UserManager<User>>()
.AddDefaultTokenProviders();

// Auth0 JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = "https://dev-vxfnqxnbydwtkwan.us.auth0.com/"; // Auth0 domain
    options.Audience = "https://localhost:44385/"; // API identifier or audience
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bINs4ZHwuFqEIdw1MhzgZlV7dlbR1QPa")), // Auth0 secret
        ValidateIssuer = true,
        ValidIssuer = "https://dev-vxfnqxnbydwtkwan.us.auth0.com/",
        ValidateAudience = true,
        ValidAudience = "https://localhost:44385/",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS configuration
builder.Services.AddCors();

// Configure API Behavior
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new { Errors = errors };
        return new BadRequestObjectResult(toReturn);
    };
});

// Authorization policies
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
    opt.AddPolicy("PlayerPolicy", policy => policy.RequireRole("Player"));
    opt.AddPolicy("AdminOrManagerPolicy", policy => policy.RequireRole("Admin", "Manager"));
    opt.AddPolicy("AdminAndManagerPolicy", policy => policy.RequireRole("Admin").RequireRole("Manager"));
    opt.AddPolicy("AllRolePolicy", policy => policy.RequireRole("Admin", "Manager", "Player"));
    opt.AddPolicy("AdminEmailPolicy", policy => policy.RequireClaim(ClaimTypes.Email, "admin@gmail.com"));
    opt.AddPolicy("ManagerEmailPolicy", policy => policy.RequireClaim(ClaimTypes.Email, "manager@gmail.com"));
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();  // Enable authorization middleware

app.MapControllers();

// Database seeding during app startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var contextSeedService = services.GetRequiredService<ContextSeedService>();
        await contextSeedService.InitializeContext();
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occurred while seeding the database.");
    }
}

app.Run();
