using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class RoleEndpoints
    {
        public static void MapRoleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/roles")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/", GetRolesAsync)
                .WithName("getRoles");

            group.MapGet("/{id}", GetRolebyIdAsync)
                .WithName("getRoleById");

            group.MapPost("/", AddRoleAsync)
                .WithName("addRole");

            group.MapPut("/", UpdateRoleAsync)
                .WithName("updateRole");

            group.MapDelete("/{id}", DeleteRoleAsync)
                .WithName("deleteRole");

            group.MapPatch("/assign-permission", AssignPermissionAsync)
                .WithName("assignPermission");

            group.MapPatch("/remove-permission", RemovePermissionAsync)
                .WithName("removePermission");
        }
        private static async Task<Result<List<Role>>> GetRolesAsync([FromServices] IRoleService roleService)
        {
            return await roleService.GetRolesAsync();
        }
        private static async Task<Result<Role>> GetRolebyIdAsync([FromServices] IRoleService roleService, Guid roleId)
        {
            return await roleService.GetRolesbyIdAsync(roleId);
        }
        private static async Task<Result> AddRoleAsync([FromServices] IRoleService roleService, [FromBody] string role)
        {
            return await roleService.AddRoleAsync(role);
        }
        private static async Task<Result<Role>> UpdateRoleAsync([FromServices] IRoleService roleService, [FromBody] Role role)
        {
            return await roleService.UpdateRoleAsync(role);
        }
        private static async Task<Result> DeleteRoleAsync([FromServices] IRoleService roleService, Guid id)
        {
            return await roleService.DeleteRoleAsync(id);
        }
        private static async Task<Result> AssignPermissionAsync([FromServices] IRoleService roleService, [FromBody] PermissionRolesDto permissionRoles)
        {
            return await roleService.AssignPermissionAsync(permissionRoles);
        }
        private static async Task<Result> RemovePermissionAsync([FromServices] IRoleService roleService, [FromBody] PermissionRolesDto permissionRoles)
        {
            return await roleService.RemovePermissionAsync(permissionRoles);
        }
    }
}
