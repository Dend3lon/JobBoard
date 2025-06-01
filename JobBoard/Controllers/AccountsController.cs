using DomainData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobBoard.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace JobBoard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly JobBoardContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IdentityService _identitityService;
        public AccountsController(
            JobBoardContext ctx,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IdentityService identitityService)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _identitityService = identitityService;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterUser registerUser)
        {
            var identity = new IdentityUser { Email = registerUser.Email, UserName = registerUser.Email};
            var createIdentity = await _userManager.CreateAsync(identity, registerUser.Password);

            var newClaims = new List<Claim>
            {
                new("FirstName", registerUser.FirstName),
                new("LastName", registerUser.LastName)
            };

            await _userManager.AddClaimsAsync(identity, newClaims); 

            if (registerUser.Role == Role.Admin)
            {
                var role = new IdentityRole { Name = "Admin" };
                if (role == null)
                {
                    role = new IdentityRole { Name = "Admin" };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(identity, role.Name);

                newClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            else if (registerUser.Role == Role.Worker)
            {
                var role = new IdentityRole { Name = "Worker" };
                if (role == null)
                {
                    role = new IdentityRole { Name = "Worker" };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(identity, role.Name);
                newClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            else if (registerUser.Role == Role.Employer)
            {
                var role = new IdentityRole { Name = "Employer" };
                if (role == null)
                {
                    role = new IdentityRole { Name = "Employer" };
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(identity, role.Name);
                newClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, identity.Email ?? throw new InvalidOperationException()),
                new(JwtRegisteredClaimNames.Email, identity.Email ?? throw new InvalidOperationException()),
            });

            claimsIdentity.AddClaims(newClaims);

            var token = _identitityService.CreateSecurityToken(claimsIdentity);
            var response = new AuthenticationResult(_identitityService.WriteToken(token));

            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginUser login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var claims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, user.Email ?? throw new InvalidOperationException()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException()),
            });

            claimsIdentity.AddClaims(claims);

            foreach (var role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = _identitityService.CreateSecurityToken(claimsIdentity);
            var response = new AuthenticationResult(_identitityService.WriteToken(token));
            return Ok(response);

        }
    }

    public enum Role
    {
        Admin,
        Worker,
        Employer
    }
    public record RegisterUser(string Email, string Password, string FirstName, string LastName, Role Role);
    public record LoginUser(string Email, string Password);
    public record AuthenticationResult(string Token);
}
