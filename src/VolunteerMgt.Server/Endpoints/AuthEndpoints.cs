using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.Wrapper;
using VolunteerMgt.Server.Models.User;

namespace VolunteerMgt.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/authentication")
            .WithOpenApi();

        group.MapPost("/login", GetAuthTokenAsync)
            .WithName("login");

        group.MapPost("/register", RegisterAsync)
            .WithName("register");
    }

    private static async Task<Result<TokenResponse>> GetAuthTokenAsync(
        [FromServices] IAuthService authService,
        [FromBody] TokenRequest request)
    {
        return await authService.LoginAsync(request);
    }

    private static async Task<Result<TokenResponse>> RegisterAsync(
        [FromServices] IAuthService authService,
        [FromBody] UserModel request)
    {
        return await authService.RegisterAsync(request);
    }
}
