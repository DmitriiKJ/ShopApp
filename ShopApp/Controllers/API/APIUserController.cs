using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public APIUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email or password are require");
            }
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok("User was register...");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email or password are require");
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                return Ok("Auth successfully...");
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
    }
}
