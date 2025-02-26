using System.Security.Claims;
using VolunteerMgt.Server.Helpers;

namespace VolunteerMgt.Server.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserName(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Name);

    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email);

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst(Claims.Fullname)?.Value;

    public static string? GetSurname(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Surname)?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetImageUrl(this ClaimsPrincipal principal)
        => principal.FindFirstValue(Claims.ImageUrl);

    public static List<string>? GetRoles(this ClaimsPrincipal principal)
        => principal.FindAll(ClaimTypes.Role).Select(s => s.Value).ToList();

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;
}