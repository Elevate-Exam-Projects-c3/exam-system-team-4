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
    {
        var now = DateTimeOffset.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64)
        };
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(_options.DurationInMinutes).UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256));

        return new TokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            _options.DurationInMinutes * 60,
            Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64)),
            now.AddDays(7));
    }

    public string HashRefreshToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
