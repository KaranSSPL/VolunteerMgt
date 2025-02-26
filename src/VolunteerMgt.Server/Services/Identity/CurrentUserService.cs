using System.Security.Claims;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Extensions;

namespace VolunteerMgt.Server.Services.Identity;
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user = null;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
            : _userId;

    public string? GetUserName() =>
        IsAuthenticated()
            ? _user?.GetUserName()
            : null;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user?.GetEmail()
            : null;

    public string? GetUserPhone() =>
        IsAuthenticated()
            ? _user?.GetPhoneNumber()
            : null;

    public string? GetUserFullName() =>
        IsAuthenticated()
            ? _user?.GetFullName()
            : null;

    public List<string>? GetRoles() =>
        IsAuthenticated()
            ? _user?.GetRoles()
            : null;

    public bool IsAuthenticated() => _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) => _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() => _user?.Claims;

    public string AppBaseUrl => httpContextAccessor.HttpContext != null ? $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}" : "";

    public string GetIp() =>
       httpContextAccessor.HttpContext?.Request.Headers != null && httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For") ? httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].Count > 0 ? httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString() : "N/A" : httpContextAccessor.HttpContext?.Connection.RemoteIpAddress != null ? httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() : "N/A";

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null) throw new Exception("Method reserved for in-scope initialization");
        _user = user;
    }
}