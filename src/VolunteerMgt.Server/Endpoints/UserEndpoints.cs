using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models.Register;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/user")
                .WithOpenApi();

            group.MapPost("/register-user", AddUserAsync)
                .WithName("registerUser")
                .RequireAuthorization();

            group.MapGet("/delete-user/{id}", DeleteUserAsync)
                .WithName("deleteUser")
                .RequireAuthorization();
        }
        private static async Task<Result> AddUserAsync([FromServices] IUserService userService, [FromBody] RegisterModel model)
        {
            return await userService.AddUserAsync(model);
        }
        private static async Task<Result> DeleteUserAsync([FromServices] IUserService userService, [FromQuery] string Id)
        {
            return await userService.DeleteUserAsync(Id);
        }
    }
}
