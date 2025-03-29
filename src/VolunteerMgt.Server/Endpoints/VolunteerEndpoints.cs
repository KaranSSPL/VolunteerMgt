using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.User;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class VolunteerEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/users")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/", GetUsersAsync)
                .WithName("getUsers");

            group.MapGet("/{id}", GetUserbyIdAsync)
                .WithName("getUserById");

            group.MapGet("/{id}/roles", GetUserRolesAsync)
                .WithName("getUserRoles");

            group.MapPost("/", AddUserAsync)
                .WithName("AddUser")
                .AllowAnonymous();

            group.MapPut("/", UpdateUserAsync)
                .WithName("updateUser");

            group.MapDelete("/{id}", DeleteUserAsync)
                .WithName("deleteUser");

            group.MapPatch("/assign-role", AssignRoleAsync)
                .WithName("assignUserRoles");

            group.MapPatch("/remove-role", RemoveRoleAsync)
                .WithName("removeUserRoles");

            group.MapPut("/change-password", ChangePasswordAsync)
                .WithName("changePassword");
        }
        private static async Task<Result<List<UserWithId>>> GetUsersAsync([FromServices] IUserService userService)
        {
            return await userService.GetUsersAsync();
        }
        private static async Task<Result<UserWithId>> GetUserbyIdAsync([FromServices] IUserService userService, Guid id)
        {
            return await userService.GetUserByIdAsync(id);
        }
        private static async Task<Result<EditUserModel>> UpdateUserAsync([FromServices] IUserService userService, [FromBody] EditUserModel model)
        {
            return await userService.UpdateUserAsync(model);
        }

        private static async Task<Result> ChangePasswordAsync([FromServices] IUserService userService, [FromBody] ChangePasswordModel model)
        {
            return await userService.ChangePasswordAsync(model);
        }
        private static async Task<Result<RegisterUserModel>> AddUserAsync([FromServices] IUserService userService, [FromBody] RegisterUserModel model)
        {
            return await userService.AddUserAsync(model);
        }
        private static async Task<Result> GetUserRolesAsync([FromServices] IUserService userService, Guid id)
        {
            return await userService.GetUserRolesAsync(id);
        }
        private static async Task<Result> AssignRoleAsync([FromServices] IUserService userService, [FromBody] UserRoleMapping userRole)
        {
            return await userService.AssignRoleAsync(userRole);
        }
        private static async Task<Result> RemoveRoleAsync([FromServices] IUserService userService, [FromBody] UserRoleMapping userRole)
        {
            return await userService.RemoveRoleAsync(userRole);
        }
        private static async Task<Result> DeleteUserAsync([FromServices] IUserService userService, string id)
        {
            return await userService.DeleteUserAsync(id);
        }
    }
}
