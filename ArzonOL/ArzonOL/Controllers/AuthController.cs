#pragma warning disable
using ArzonOL.Dtos.AuthDtos;
using ArzonOL.Entities;
using ArzonOL.Enums;
using ArzonOL.Services.AuthService.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArzonOL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IRegisterService _registerService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailSender _mailSender;

        public AuthController(ILoginService loginService,
                              IRegisterService registerService,
                              UserManager<UserEntity> userManager,
                              IConfiguration configuration,
                              IMailSender mailSender)
        {
            _loginService = loginService;
            _registerService = registerService;
            _userManager = userManager;
            _configuration = configuration;
            _mailSender = mailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var token = await _loginService.LogInAsync(loginDto.UserName!, loginDto.Password!);

                if (string.IsNullOrEmpty(token))
                    return BadRequest("Username or password is incorrect");

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(7), // Set expiration date to 7 days from now
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return Ok(token);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var result = await _registerService.RegisterAsync(registerDto.UserName!, registerDto.Password!, "User", registerDto.Email!);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(result);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("signin-google")]
        public async Task<IActionResult> SignInWithGoogle(LoginWithGoogle loginWithGoogle)
        {
            var payload = await _loginService.VerifyGoogleToken(loginWithGoogle.Provider!, loginWithGoogle.IdToken!);

            if (payload is null)
                return BadRequest(new { Succeeded = false });

            var info = new UserLoginInfo(loginWithGoogle.Provider!, payload.Subject, loginWithGoogle.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user is null)
                {
                    user = new UserEntity { Email = payload.Email, UserName = payload.Email };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return BadRequest(new
                        {
                            Succeeded = false,
                            Errors = new List<string>(result.Errors.Select(x => x.Description)) { "Invalid External Auth" }
                        });

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(await _userManager.FindByEmailAsync(user.Email));
                    _mailSender.Send(user.Email, "Email Confirmation Message", @$"
                        <html>
                        <head> 
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        </head>
                        <body>
                            <h3>Your confirmation message</h3>
                            <p>Visit through this link to confirm your email: </p>
                            <form method=""post"" action=""{_configuration.GetSection("Urls")["ConfirmUrl"]}"">
                                <input type=""hidden"" value=""{token}"" name=""token""/>
                                <input type=""hidden"" value=""{user.Email}"" name=""email""/>
                                <button type=""submit"" style=""background-color:#0669B4; color: white; padding: 30px; margin: 50px; width:100px; height: 30px"">
                                    Confirm
                                </button>
                            </form>
                            <h5 stype=""padding: 10px;"">{token}</h5>
                        </body>
                        </html>
                    ");

                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            var userRole = await _userManager.GetRolesAsync(identityUser);
            var jwtToken = _loginService.CreateJwtToken(identityUser.UserName, identityUser.Email, userRole[0], identityUser.Id);
            return Ok(new { Succeeded = true, Token = jwtToken });
        }

        [HttpPost("merchant/register")]
        public async Task<IActionResult> RegisterMerchantAsync(RegisterDto registerDto)
        {
            try
            {
                var result = await _registerService.RegisterAsync(registerDto.UserName!, registerDto.Password!, "Merchant", registerDto.Email!);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(result.Succeeded);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogOutDto loginDto)
        {
            try
            {
                var result = await _loginService.LogOutAsync(loginDto.Id, loginDto.Password!);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(result.Succeeded);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (Guid.Empty == changePasswordDto.Id)
                    return BadRequest(IdentityResult.Failed(new IdentityError { Code = EErrorType.ServerError.ToString(), Description = "Id can't be null here" }));

                return Ok(await _registerService.ChangePasswordAsync(changePasswordDto.Id, changePasswordDto.OldPassword!, changePasswordDto.NewPassword!));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("changeUsername")]
        public async Task<IActionResult> ChangeUserName(ChangeUsernameDto changeUsernameDto)
        {
            try
            {
                var result = await _registerService.ChangeUsernameAsync(changeUsernameDto.Id, changeUsernameDto.NewUserName!);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(result.Succeeded);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
