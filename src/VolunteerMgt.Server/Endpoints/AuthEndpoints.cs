using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service.Identity;
using VolunteerMgt.Server.Models.Auth;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/authentication")
            .WithOpenApi();

        group.MapPost("/", GetAuthTokenAsync)
            .WithName("token");

        group.MapPost("/forgot-password", ForgotPasswordAsync)
            .WithName("forgotPassword");

        group.MapGet("/reset-password", ResetPasswordAsync)
            .WithName("resetPassword");

        group.MapPost("/reset-password", ResetPasswordPostAsync)
            .WithName("resetPasswordPost");
    }

    private static async Task<Result<TokenResponse>> GetAuthTokenAsync([FromServices] IAuthService authService, [FromBody] TokenRequest request)
    {
        return await authService.GetTokenAsync(request);
    }
    private static async Task<Result> ForgotPasswordAsync([FromServices] IAuthService authService, [FromBody] ForgotPasswordModel model)
    {
        return await authService.ForgotPasswordAsync(model);
    }
    private static async Task<Result> ResetPasswordAsync([FromServices] IAuthService authService, [FromQuery] string email, [FromQuery] string token)
    {
        return await authService.ResetPasswordAsync(email, token);
    }
    private static async Task<Result> ResetPasswordPostAsync([FromServices] IAuthService authService, [FromBody] ResetPasswordModel model)
    {
        return await authService.ResetPasswordPostAsync(model);
    }
}
