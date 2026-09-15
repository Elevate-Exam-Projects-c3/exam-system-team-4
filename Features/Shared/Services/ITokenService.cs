using System.Text.Json.Serialization;
using exam_system.Domain.Entities.Identity;

namespace exam_system.Features.Shared.Services;

public interface ITokenService
{
    TokenResult GenerateTokens(ApplicationUser user, IEnumerable<string> roles);
    TokenResult GenerateTokens(string userId, string email, IEnumerable<string> roles, Guid? studentId);
    string HashRefreshToken(string token);
}

public sealed record TokenResult(string AccessToken,int ExpiresIn,
    [property: JsonIgnore] string RefreshToken,
    [property: JsonIgnore] DateTimeOffset RefreshTokenExpiresAt)
{
    public string TokenType => "Bearer";
}
