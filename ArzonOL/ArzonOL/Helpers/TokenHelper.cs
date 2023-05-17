using System.IdentityModel.Tokens.Jwt;

namespace ArzonOL.Helpers;

public class TokenHelper
{
    public static string GetUserIdFromToken(HttpContext context, string cookieName)
    {
        string token = context.Request.Cookies[cookieName];

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var idClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Id");
            if (idClaim != null)
            {
                string userId = idClaim.Value;
                return userId;
            }
        }

        return null;
    }
}