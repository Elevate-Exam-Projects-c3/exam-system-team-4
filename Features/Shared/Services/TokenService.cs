using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using exam_system.Domain.Entities.Identity;
using exam_system.Helper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace exam_system.Features.Shared.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtOption _options;

    public TokenService(IOptions<JwtOption> options)
    {
        _options = options.Value;
    }

    public TokenResult GenerateTokens(ApplicationUser user, IEnumerable<string> roles)
        => GenerateTokens(user.Id, user.Email!, roles, user.Student?.Id);

    public TokenResult GenerateTokens(string userId,string email,IEnumerable<string> roles,Guid? studentId)
    {
        // 1. تحديد وقت الإنشاء ومواعيد انتهاء الصلاحية

        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.DurationInMinutes);

        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(7);

        var expiresInSeconds = _options.DurationInMinutes * 60;

        // 2. تجهيز بيانات المستخدم التي ستوضع داخل الـ JWT
        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),

            new Claim(JwtRegisteredClaimNames.Email, email),

            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

            new Claim(JwtRegisteredClaimNames.Iat,issuedAt, ClaimValueTypes.Integer64)
        };
    

        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        if (studentId.HasValue)
        {
            claims.Add(new Claim("studentId", studentId.Value.ToString()));
        }

        // 3. تجهيز المفتاح وطريقة توقيع الـ JWT
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key))
            , SecurityAlgorithms.HmacSha256);

        // 4. إنشاء الـ JWT
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTimeOffset.Now.UtcDateTime,
            expires: accessTokenExpiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        // 5. تحويل الـ JWT إلى النص الذي سيُرسل للعميل
        var tokenHandler = new JwtSecurityTokenHandler();

        var accessToken = tokenHandler.WriteToken(jwt);

        // 6. إنشاء Refresh Token عشوائي

        var refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

        // 7. تجميع النتائج وإرجاعها للكود الذي طلب التوكن
        return new TokenResult( AccessToken: accessToken, ExpiresIn: expiresInSeconds,
            RefreshToken: refreshToken,
            RefreshTokenExpiresAt: refreshTokenExpiresAt);
    }

    public string HashRefreshToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
