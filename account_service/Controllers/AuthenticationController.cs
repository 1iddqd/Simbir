using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static account_service.Program;
using account_service.Database;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace account_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILogger<AuthenticationController> _logger;
        private AccountApplicationContext _dbcontext = new AccountApplicationContext();
        private readonly JWTSettings _options;

        public AuthenticationController(ILogger<AuthenticationController> logger, IOptions<JWTSettings> optAccess)
        {
            _logger = logger;
            _options = optAccess.Value;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var username = request.username;
            var password = request.password;
            var lastName = request.lastName;
            var firstName = request.firstName;

            var existingUser = _dbcontext.Accounts.Where(x => x.username == username).FirstOrDefault();
            if (existingUser != null)
            {
                return Conflict("User with this login already exists");
            }

            var lastUser = await _dbcontext.Accounts.OrderByDescending(x => x.id).FirstOrDefaultAsync();
            int newId = (lastUser != null) ? lastUser.id + 1 : 1;

            var newUser = new Accounts
            {
                id = newId,
                firstName = firstName,
                lastName = lastName,
                username = username,
                password = password
            };

            _dbcontext.Accounts.Add(newUser);
            await _dbcontext.SaveChangesAsync();

            return Ok("Account created");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var username = request.username;
            var password = request.password;

            var user = _dbcontext.Accounts
                .Where(x => x.username == username && x.password == password)
                .FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Incorrect login or password");
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = GenerateRefreshToken();
            var refreshTokenObject = new RefreshTokens
            {
                accounts = user,
                token = refreshToken
            };

            _dbcontext.RefreshTokens.Add(refreshTokenObject);
            _dbcontext.SaveChanges();

            var handler = new JwtSecurityTokenHandler();
            return Ok(new
            {
                AccessToken = handler.WriteToken(jwt),
                RefreshToken = refreshToken
            });
        }
        [HttpPut("SignOut")]
        [Authorize]
        public IActionResult AccountSignOut()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            RevokedTokens.RevokeToken(token);

            return Ok("Succesesfully signed out");
        }


        [HttpGet("Validate")]
        public IActionResult Validate()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (token == null)
            {
                return BadRequest("Invalid token");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                return Ok("Token is valid");
            }
            catch (Exception)
            {
                return Unauthorized("Token is not valid");
            }
        }


        [HttpPost("Refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.token))
                {
                    return BadRequest("Refresh token is missing.");
                }

                var token = _dbcontext.RefreshTokens.FirstOrDefault(x => x.token == request.token);
                if (token == null)
                {
                    return BadRequest("Invalid refresh token.");
                }

                var user = _dbcontext.Accounts.FirstOrDefault(x => x.id == token.accounts.id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
                var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                var refreshToken = GenerateRefreshToken();
                var refreshTokenObject = new RefreshTokens
                {
                    accounts = user,
                    token = refreshToken
                };

                var tokensToRemove = _dbcontext.RefreshTokens.Where(x => x.token == request.token).ToList();
                
                _dbcontext.RefreshTokens.RemoveRange(tokensToRemove);
                _dbcontext.RefreshTokens.Add(refreshTokenObject);
                _dbcontext.SaveChanges();

                var handler = new JwtSecurityTokenHandler();
                return Ok(new
                {
                    AccessToken = handler.WriteToken(jwt),
                    RefreshToken = refreshToken
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }

    public static class RevokedTokens
    {
        private static HashSet<string> _tokens = new HashSet<string>();

        public static void RevokeToken(string token)
        {
            _tokens.Add(token);
        }

        public static bool IsTokenRevoked(string token)
        {
            return _tokens.Contains(token);
        }
    }
    public class SignInRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class SignUpRequest
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RefreshRequest
    {
        public string token { get; set; }
    }
}
