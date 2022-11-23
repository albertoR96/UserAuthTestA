using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserAuthTestA.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UserAuthTestA.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            this.configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(UserCredential credentials)
        {
            var user = new IdentityUser { UserName = credentials.Email, Email = credentials.Email };
            var result = await _userManager.CreateAsync(user, credentials.Password);
            if (result.Succeeded)
            {
                return await BuildToken(credentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(UserCredential userCredential)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredential.Email, userCredential.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return await BuildToken(userCredential);
            }
            else
            {
                return BadRequest("bad login");
            }
        }

        [HttpGet("renew_token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthResponse>> RenreToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credentials = new UserCredential() { Email = email };
            return await BuildToken(credentials);
        }

        private async Task<AuthResponse> BuildToken(UserCredential userCredentials)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentials.Email)
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDB = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);
            return new AuthResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        [HttpPost("giveAdmin")]
        public async Task<ActionResult> GiveAdmin(GiveAdminRequest giveAdminRequest)
        {
            var user = await _userManager.FindByEmailAsync(giveAdminRequest.Email);
            await _userManager.AddClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }

        [HttpPost("removeAdmin")]
        public async Task<ActionResult> RemoveAdmin(RemoveAdminRequest removeAdminRequest)
        {
            var user = await _userManager.FindByEmailAsync(removeAdminRequest.Email);
            await _userManager.RemoveClaimAsync(user, new Claim("IsAdmin", "1"));
            return NoContent();
        }
    }
}
