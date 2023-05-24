#pragma warning disable
using ArzonOL.Entities;
using ArzonOL.Enums;
using ArzonOL.Services.AuthService.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ArzonOL.Services.AuthService
{
    public class RegisterService : IRegisterService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly ILogger<RegisterService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterService(UserManager<UserEntity> userManager,
                               SignInManager<UserEntity> signInManager,
                               ILogger<RegisterService> logger,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid id, string oldPassword, string newPassword)
        {
            _logger.LogInformation("Changing password");

            try
            {
                var existUser = await _userManager.FindByIdAsync(id.ToString());

                if (existUser is null)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ServerError.ToString(), Description = "This user was not found" });

                var checkedPassword = await _userManager.CheckPasswordAsync(existUser, oldPassword);

                if (!checkedPassword)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ClientError.ToString(), Description = "The old password is incorrect" });

                var changePasswordResult = await _userManager.ChangePasswordAsync(existUser, oldPassword, newPassword);

                if (!changePasswordResult.Succeeded)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ServerError.ToString(), Description = string.Join(", ", changePasswordResult.Errors) });

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change password");
                throw new Exception("Failed to change password");
            }
        }

        public async Task<IdentityResult> ChangeUsernameAsync(Guid id, string newUsername)
        {
            _logger.LogInformation("Changing username with id " + id.ToString());

            try
            {
                var existUser = await _userManager.FindByIdAsync(id.ToString());

                if (existUser is null)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ServerError.ToString(), Description = "This user was not found" });

                var checkedPassword = await _userManager.CheckPasswordAsync(existUser, existUser.PasswordHash);

                if (!checkedPassword)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ClientError.ToString(), Description = "The password is incorrect" });

                existUser.UserName = newUsername;

                var changeUsernameResult = await _userManager.UpdateAsync(existUser);

                if (!changeUsernameResult.Succeeded)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ServerError.ToString(), Description = string.Join(", ", changeUsernameResult.Errors) });

                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to change username");
                throw new Exception("Failed to change username");
            }
        }

        public async Task<IdentityResult> RegisterAsync(string username, string password, string role, string email)
        {
            _logger.LogInformation("Registering user {username}", username);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(email))
                return IdentityResult.Failed(new IdentityError { Code = EErrorType.ClientError.ToString(), Description = "Username, password, role, or email is empty" });

            try
            {
                var user = new UserEntity { UserName = username, Email = email };
                var userCreateResult = await _userManager.CreateAsync(user, password);

                if (!userCreateResult.Succeeded)
                    return IdentityResult.Failed(new IdentityError { Code = EErrorType.ClientError.ToString(), Description = string.Join(", ", userCreateResult.Errors) });

                if (await _roleManager.RoleExistsAsync(role))
                    await _userManager.AddToRoleAsync(user, role);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register user");
                throw new Exception("Failed to register user");
            }
        }
    }
}
