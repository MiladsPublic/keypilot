using System.Security.Claims;

namespace KeyPilot.Api.Auth;

public static class CurrentUserExtensions
{
    public static string? GetCurrentUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");
    }
}
