using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Yami33.Utility;

namespace Yami33.Services
{


    public class MockAuthService : IAuthService
    {
        public Task<string?> LoginAsync(string username, string password)
        {
            // می‌توانی شرایط مختلف تست را شبیه‌سازی کنی
            if (username == "admin" && password == "123")
            {


                string token = GenerateMockJwt("Admin", "Admin");

                return Task.FromResult<string?>(token);
            }

            return Task.FromResult<string?>(null);
        }


        private string GenerateMockJwt(string username, string role)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(ClaimTypes.Role, role)
    };

            // برای Mock، از یک کلید ساده استفاده می‌کنیم
           // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dummy-secret-1234567890"));

            var key = new SymmetricSecurityKey(
       Encoding.UTF8.GetBytes("MySuperLongSecretKey_ForJWT_Token@2025") // حداقل 32 کاراکتر
   );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MockIssuer",
                audience: "MockAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
