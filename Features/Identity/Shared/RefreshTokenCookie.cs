using Microsoft.AspNetCore.Http;

namespace exam_system.Features.Identity.Shared;

public static class RefreshTokenCookie
{
    private const string CookieName = "refreshToken";
    private const string CookiePath = "/api/auth";

    public static string? Read(HttpRequest request) => request.Cookies[CookieName];

    public static void Append(HttpResponse response, string token, DateTimeOffset expiresAt)
    {
        var options = CreateOptions();
        options.Expires = expiresAt;
        response.Cookies.Append(CookieName, token, options);
    }

    public static void Delete(HttpResponse response)
        => response.Cookies.Delete(CookieName, CreateOptions());

    private static CookieOptions CreateOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Path = CookiePath
    };
}
