using Azure;
using GrpcJwtService.Protos;

namespace Authentication.Services
{
    public interface IJwtManager
    {
        Task<bool> SetJwtAsync(string userId, string username, string role, string email, HttpContext httpContext);
        void RemoveCookieAndSession(HttpContext httpContext);
    }

    public class JwtManager(JwtTokenService.JwtTokenServiceClient jwtClient) : IJwtManager
    {
        private readonly JwtTokenService.JwtTokenServiceClient _jwtClient = jwtClient;

        public async Task<bool> SetJwtAsync(string userId, string username, string role, string email, HttpContext httpContext)
        {
            var jwtRequest = new JwtRequest
            {
                Userid = userId,
                Username = username,
                Email = email,
                Role = role
            };
            var jwtReply = await _jwtClient.GenerateTokenAsync(jwtRequest);

            if (jwtReply.Success)
            {
                httpContext.Session.SetString("JwtToken", jwtReply.Message);
                httpContext.Response.Cookies.Append(
                    "JwtToken",
                    jwtReply.Message,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
                );
                return true;
            }
            return false;

        }

        public void RemoveCookieAndSession(HttpContext httpContext)
        {
            httpContext.Session.Remove("JwtToken");
            httpContext.Response.Cookies.Delete(
                "JwtToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                }
            );
        }
    }
}
