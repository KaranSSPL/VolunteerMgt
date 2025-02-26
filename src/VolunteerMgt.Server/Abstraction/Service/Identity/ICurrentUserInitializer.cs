using System.Security.Claims;

namespace VolunteerMgt.Server.Abstraction.Service.Identity;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);
}