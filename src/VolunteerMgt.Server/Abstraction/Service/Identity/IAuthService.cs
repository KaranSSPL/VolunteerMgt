using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service.Identity;

public interface IAuthService : IScopedService
{
    Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request);
}
