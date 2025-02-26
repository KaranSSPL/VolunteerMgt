using System.Security.Claims;
using VolunteerMgt.Server.Abstraction.Service.Common;

namespace VolunteerMgt.Server.Abstraction.Service.Identity;

public interface ICurrentUserService : ITransientService
{
    string? Name { get; }

    Guid GetUserId();

    string? GetUserEmail();

    string? GetUserName();

    string? GetUserPhone();

    string? GetUserFullName();

    List<string>? GetRoles();

    bool IsInRole(string role);

    bool IsAuthenticated();

    IEnumerable<Claim>? GetUserClaims();

    string AppBaseUrl { get; }

    string? GetIp();
}