using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
namespace ArzonOL.Services.AuthService.Interfaces;

public interface ILoginService
{
    Task<string> LogInAsync(string username, string password);
    Task<IdentityResult> LogOutAsync(Guid id, string password);
    string CreateJwtToken(string username, string email, string role, string userId);
    bool ValidateJwtToken(string token);
    public Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string provider, string idToken);
}