using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.PermissionRoles;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class PermissionEndpoints
    {
        public static void MapPermissionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/permission")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/get-permissions", GetPermissionAsync)
            .WithName("getPermission");

            group.MapPost("/add-permission", AddPermissionAsync)
                .WithName("addPermission");

            group.MapGet("/get-permission-roles/{id}", GetPermissionRolesAsync)
                .WithName("getPermissionRole");

            group.MapPut("/update-permission", UpdatePermissionAsync)
                .WithName("updatePermission");

            group.MapDelete("/delete-permission", DeletePermissionAsync)
                .WithName("deletePermission");

            group.MapPost("/add-permission-roles", AssignPermissionRoleAsync)
                .WithName("assignPermissionRoles");

            group.MapPost("/remove-permission-roles", RemovePermissionRoleAsync)
                .WithName("removePermissionRoles");
        }
        private static async Task<Result> AddPermissionAsync([FromServices] IPermissionService permissionService, [FromBody] Permission permission)
        {
            return await permissionService.AddPermissionAsync(permission);
        }
        private static async Task<Result<List<Permission>>> GetPermissionAsync([FromServices] IPermissionService permissionService)
        {
            return await permissionService.GetPermissionAsync();
        }
        private static async Task<Result<List<Permission>>> GetPermissionRolesAsync([FromServices] IPermissionService permissionService, [FromBody] Guid id)
        {
            return await permissionService.GetPermissionRolesAsync(id);
        }
        private static async Task<Result<Permission>> UpdatePermissionAsync([FromServices] IPermissionService permissionService, [FromBody] Permission permission)
        {
            return await permissionService.UpdatePermissionAsync(permission);
        }
        private static async Task<Result> DeletePermissionAsync([FromServices] IPermissionService permissionService, Guid id)
        {
            return await permissionService.DeletePermissionAsync(id);
        }
        private static async Task<Result> AssignPermissionRoleAsync([FromServices] IPermissionService permissionService, [FromBody] PermissionRolesModel permissionRoles)
        {
            return await permissionService.AssignPermissionRoleAsync(permissionRoles);
        }
        private static async Task<Result> RemovePermissionRoleAsync([FromServices] IPermissionService permissionService, [FromBody] PermissionRolesModel permissionRoles)
        {
            return await permissionService.RemovePermissionRoleAsync(permissionRoles);
        }
    }
}