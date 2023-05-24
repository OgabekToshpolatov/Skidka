#pragma warning disable
using ArzonOL.Entities;
using ArzonOL.Enums;
using ArzonOL.Services.AuthService.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ArzonOL.Services.AuthService
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<LoginService> _logger;
        private readonly IConfiguration _configuration;

        public LoginService(UserManager<UserEntity> userManager,
                            SignInManager<UserEntity> signInManager,
                            ILogger<LoginService> logger,
                            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        public string CreateJwtToken(string username, string email, string role, string userId)
        {
            _logger.LogInformation("Creating JWT token for user {username}", username);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Username or password is empty");
                return string.Empty;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("Id", userId),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, email)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration.GetSection("Jwt")["Issuer"];
            var audience = _configuration.GetSection("Jwt")["Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> LogInAsync(string username, string password)
        {
            _logger.LogInformation("Logging in user {username}", username);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Username or password is empty");
                return string.Empty;
            }

            var existingUser = await _userManager.FindByNameAsync(username);

            if (existingUser is null)
            {
                _logger.LogInformation("User not found");
                return string.Empty;
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(existingUser, password, false);

            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Username or password is incorrect");
                return string.Empty;
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                _logger.LogWarning("User not found in database");
                return string.Empty;
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                _logger.LogWarning("User has no roles");
                return string.Empty;
            }

            var result = CreateJwtToken(username, user.Email, roles[0], user.Id);

            if (string.IsNullOrEmpty(result))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = EErrorType.ServerError.ToString(),
                    Description = "Aniything went wrong"
                }).ToString();
            }

            return result;
        }

        public async Task<IdentityResult> LogOutAsync(Guid id, string password)
        {
            _logger.LogInformation($"Logging out user with id {id}");

            if (id == Guid.Empty)
            {
                _logger.LogWarning("Id can't be empty");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = EErrorType.ClientError.ToString(),
                    Description = "Id can't be null here"
                });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user is null)
                {
                    _logger.LogWarning("User not found");
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = EErrorType.ClientError.ToString(),
                        Description = "Bunday user topilmadi"
                    });
                }

                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out");

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while logging out");
                throw new Exception(e.Message);
            }
        }

        public bool ValidateJwtToken(string token)
        {
            _logger.LogInformation("Validating JWT token");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is empty");
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken == null)
                {
                    _logger.LogWarning("Token validation failed");
                    return false;
                }

                _logger.LogInformation("Token validation successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return false;
            }
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string provider, string idToken)
        {
            _logger.LogInformation("Started Verifying");

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Authentication:client_id"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch (Exception)
            {
                _logger.LogInformation("Failed while verifying GoogleToken");
                return null;
            }
        }
    }
}
