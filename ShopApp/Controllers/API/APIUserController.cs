using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApp.Controllers.API
{
    // This controller was created for our partners
    [Route("api/[controller]")]
    [ApiController]
    public class APIUserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public APIUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO rdto)
        {
            if (string.IsNullOrEmpty(rdto.Email) || string.IsNullOrEmpty(rdto.Password))
            {
                return BadRequest("Email or password are require");
            }

            var user = new IdentityUser { 
                UserName = rdto.Email, 
                Email = rdto.Email, 
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, rdto.Password);

            if (result.Succeeded)
            {
                return Ok("User was register...");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] LoginDTO ldto)
        {
            if (string.IsNullOrEmpty(ldto.Email) || string.IsNullOrEmpty(ldto.Password))
            {
                return BadRequest("Email or password are require");
            }

            var user = await _userManager.FindByEmailAsync(ldto.Email);
            if (user == null)
            {
                return BadRequest("Invalid email");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, ldto.Password, false);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new {Token = token});
            }

            return BadRequest("Error with auth");
        }

        public async Task<IActionResult> AccessDenied()
        {
            return BadRequest("Not authorize");
        }

        [HttpPost("role")]
        [Authorize(Roles = "Admin")] // Comment this code, if you want to add and assign role
        public async Task<IActionResult> CreateRole(string roleName)
        {
                if (string.IsNullOrEmpty(roleName))
                {
                    return BadRequest("Role name is required");
                }

                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    return BadRequest("Role already exists");
                }

                var role = new IdentityRole { Name = roleName };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return Ok($"The role {roleName} was created successfully!");
                }

                return BadRequest(result.Errors);

        }

        [HttpPost("roleassign")]
        [Authorize(Roles = "Admin")] // Comment this code, if you want to add and assign role
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
                {
                    return BadRequest("Id or role are require");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("The user not found ...");
                }

                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    return NotFound($"The role {roleName} not found ...");
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return Ok("Role was assign!");
                }
                return BadRequest(result.Errors);

        }

        [HttpPut]
        public async Task<IActionResult> Update(string oldPassword, string newEmail, string newPassword)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                var emailRes = await _userManager.SetEmailAsync(user, newEmail);
                if (!emailRes.Succeeded)
                {
                    return BadRequest("Error with change email");
                }

                var passRes = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!passRes.Succeeded)
                {
                    return BadRequest("Error with change password");
                }

                return Ok("User data was changed");
            }

            return BadRequest("You are not authotize");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                
                _userManager.DeleteAsync(user);

                return Ok("User data was changed");
            }
            return BadRequest("You are not authotize");

        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
