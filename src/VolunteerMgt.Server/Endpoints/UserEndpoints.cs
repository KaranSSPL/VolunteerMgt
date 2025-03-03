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
                .WithOpenApi()
                .RequireAuthorization();

            group.MapPost("/register-user", AddUserAsync)
                .WithName("registerUser");

            group.MapDelete("/delete-user", DeleteUserAsync)
                .WithName("deleteUser");
        }
        private static async Task<Result<RegisterModel>> AddUserAsync([FromServices] IUserService userService, [FromBody] RegisterModel model)
        {
            return await userService.AddUserAsync(model);
        }
        private static async Task<Result> DeleteUserAsync([FromServices] IUserService userService, string Id)
        {
            return await userService.DeleteUserAsync(Id);
        }
    }
}
