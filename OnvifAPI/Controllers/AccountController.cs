using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OnvifAPI.Interfaces;
using OnvifAPI.Model;
using OnvifAPI.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnvifAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [EnableCors("OpenCorsPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IConfigurationRoot _config;
        private readonly string? Secretkey;
        private readonly string? RefreshSecretKey;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;
        public int AccessTimeInMinutes { get; }
        public int RefreshTokenInDays { get; }
        public AccountController(IUserService userService,ILogger<AccountController> logger)
        {
            _logger = logger;
            _config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", true, true)
            .Build();

            Secretkey = _config["Jwt:SecretKey"];
            RefreshSecretKey = _config["Jwt:RefreshSecretKey"];
            AccessTimeInMinutes = int.Parse( _config["Jwt:AccessTimeInMinutes"]);
            RefreshTokenInDays = int.Parse(_config["Jwt:RefreshTokenInDays"]);
            _userService = userService;
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] Login model)
        {
            try
            {
                var user = _userService.Login(model.Username, model.Password);
                if (user == null)
                    return BadRequest("Invalid Username of Password");

                var accessToken = GenerateAccessToken(user.Name, user.Id.ToString(), Secretkey, DateTime.Now.AddMinutes(AccessTimeInMinutes));
                var refreshToken = GenerateAccessToken(user.Name, user.Id.ToString(), RefreshSecretKey, DateTime.Now.AddDays(RefreshTokenInDays));
                return Ok(new TokenModel { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError("Login " + ex);
                throw;
            }
           
        }

        [HttpPost("refresh")]
        public ActionResult RefreshToken(string refreshToken)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest("Invalid refresh token");
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(RefreshSecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var identity = principal.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    return Unauthorized("Invalid token");
                }

                var username = identity.FindFirst("user")?.Value;
                var userId = identity.FindFirst("id")?.Value;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid refresh token");
                }

                var newAccessToken = GenerateAccessToken(userId,username, Secretkey, DateTime.Now.AddMinutes(AccessTimeInMinutes));

                return Ok(new { AccessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError("RefreshToken " + ex);
                throw;
            }
        }

        
        private string GenerateAccessToken(string username, string userId ,string secretKey, DateTime dateTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("user",username),
                    new Claim("id",userId)
                }),
                Expires = dateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
