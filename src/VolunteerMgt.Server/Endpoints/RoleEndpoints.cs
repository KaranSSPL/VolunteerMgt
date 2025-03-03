using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models.Role;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class RoleEndpoints
    {
        public static void MapRoleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/role")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapPost("/add-role", AddRoleAsync)
                .WithName("addRole");

            group.MapGet("/get-user-roles/{id}", GetUserRolesAsync)
                .WithName("getUserRoles");

            group.MapPut("/update-role", UpdateRoleAsync)
                .WithName("updateRole");

            group.MapDelete("/delete-role", DeleteRoleAsync)
                .WithName("deleteRole");

            group.MapPost("/assign-user-roles", AssignUserRoleAsync)
                .WithName("assignUserRoles");

            group.MapPost("/remove-user-roles", RemoveUserRoleAsync)
                .WithName("removeUserRoles");

        }
        private static async Task<Result> AddRoleAsync([FromServices] IRoleService roleService, [FromBody] string role)
        {
            return await roleService.AddRoleAsync(role);
        }
        private static async Task<Result<Role>> UpdateRoleAsync([FromServices] IRoleService roleService, [FromBody] Role role)
        {
            return await roleService.UpdateRoleAsync(role);
        }
        private static async Task<Result> GetUserRolesAsync([FromServices] IRoleService roleService, Guid userId)
        {
            return await roleService.GetUserRolesAsync(userId);
        }
        private static async Task<Result> AssignUserRoleAsync([FromServices] IRoleService roleService, [FromBody] AssignUser role)
        {
            return await roleService.AssignUserRoleAsync(role);
        }
        private static async Task<Result> RemoveUserRoleAsync([FromServices] IRoleService roleService, [FromBody] AssignUser role)
        {
            return await roleService.RemoveUserRoleAsync(role);
        }
        private static async Task<Result> DeleteRoleAsync([FromServices] IRoleService roleService, Guid Id)
        {
            return await roleService.DeleteRoleAsync(Id);
        }
    }
}
