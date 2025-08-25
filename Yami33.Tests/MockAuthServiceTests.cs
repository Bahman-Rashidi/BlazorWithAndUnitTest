using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yami33.Services;

namespace Yami33.Tests
{
    public class MockAuthServiceTests
    {
        // همون کلیدی که داخل MockAuthService استفاده کرده‌ای
        private const string Secret = "MySuperLongSecretKey_ForJWT_Token@2025";

        private readonly MockAuthService _sut = new(); // System Under Test

        [Fact]
        public async Task Login_WithValidCreds_Returns_SignedJwt_WithExpectedClaims_And_1hExpiry()
        {
            // Arrange + Act
            var token = await _sut.LoginAsync("admin", "123");

            // Assert: توکن باید وجود داشته باشد
            token.Should().NotBeNullOrEmpty();

            // امضا/issuer/audience/lifetime اعتبارسنجی شوند
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(
                token!,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = "MockIssuer",
                    ValidateAudience = true,
                    ValidAudience = "MockAudience",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5)
                },
                out var validatedToken
            );

            principal.Identity?.IsAuthenticated.Should().BeTrue();
            principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be("Admin");
            principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.Should().Be("Admin");

            var jwt = (JwtSecurityToken)validatedToken;
            // الگوریتم امضا
            jwt.Header.Alg.Should().Be(SecurityAlgorithms.HmacSha256);
            // حدوداً یک ساعت اعتبار داشته باشد
            (jwt.ValidTo - DateTime.UtcNow).Should().BeCloseTo(TimeSpan.FromHours(1), TimeSpan.FromMinutes(2));
        }


        //dotnet test --filter "FullyQualifiedName~MockAuthServiceTests.LoginAsync_InvalidCreds_ReturnsNull"
        [Theory]
        [InlineData("admin", "wrong")]
        [InlineData("user", "123")]
        [InlineData("foo", "bar")]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        [InlineData(" admin ", "123")]  // فاصله اضافی
        [InlineData("admin", " 123 ")]  // فاصله اضافی
        public async Task LoginAsync_InvalidCreds_ReturnsNull(string username, string password)
        {
            // Act
            var token = await _sut.LoginAsync(username, password);

            // Assert
            token.Should().BeNull();
        }

        [Fact]
        public async Task Validation_With_WrongKey_Should_Fail()
        {
            var token = await _sut.LoginAsync("admin", "123");
            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();

            Action act = () => handler.ValidateToken(
                token!,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("totally-different-key")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false
                },
                out _
            );

            act.Should().Throw<SecurityTokenInvalidSignatureException>();
        }


        //dotnet test --filter "FullyQualifiedName~MockAuthServiceTests.LoginAsync_AdminAnd123_ReturnsValidJwt"

        [Fact]
        public async Task LoginAsync_AdminAnd123_ReturnsValidJwt()
        {
            // Act
            var token = await _sut.LoginAsync("admin", "123");

            // Assert
            token.Should().NotBeNull();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // header الگوریتم امضا
            jwt.Header.Alg.Should().Be("HS256");

            // issuer / audience
            jwt.Issuer.Should().Be("MockIssuer");
            jwt.Audiences.Should().Contain("MockAudience");

            // انقضا در آینده
            jwt.ValidTo.Should().BeAfter(DateTime.UtcNow);

            // claimهای کلیدی
            jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "Admin");
        }
    }
}
