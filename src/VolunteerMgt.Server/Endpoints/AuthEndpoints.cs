using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/authentication")
            .WithOpenApi();

        group.MapPost("/", GetAuthTokenAsync)
            .WithName("token");
    }

    private static async Task<Result<TokenResponse>> GetAuthTokenAsync([FromServices] IAuthService authService, [FromBody] TokenRequest request)
    {
        return await authService.GetTokenAsync(request);
    }
}
