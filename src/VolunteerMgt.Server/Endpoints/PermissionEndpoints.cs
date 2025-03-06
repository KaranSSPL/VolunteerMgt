using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class PermissionEndpoints
    {
        public static void MapPermissionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/permissions")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/", GetPermissionAsync)
            .WithName("getPermission");

            group.MapGet("/{id}", GetPermissionByIdAsync)
                .WithName("getPermissionById");

            group.MapPost("/", AddPermissionAsync)
                .WithName("addPermission");

            group.MapPut("/", UpdatePermissionAsync)
                .WithName("updatePermission");

            group.MapDelete("/{id}", DeletePermissionAsync)
                .WithName("deletePermission");

        }
        private static async Task<Result> AddPermissionAsync([FromServices] IPermissionService permissionService, [FromBody] Permission permission)
        {
            return await permissionService.AddPermissionAsync(permission);
        }
        private static async Task<Result<List<Permission>>> GetPermissionAsync([FromServices] IPermissionService permissionService)
        {
            return await permissionService.GetPermissionAsync();
        }
        private static async Task<Result<Permission>> GetPermissionByIdAsync([FromServices] IPermissionService permissionService, Guid id)
        {
            return await permissionService.GetPermissionByIdAsync(id);
        }
        private static async Task<Result<Permission>> UpdatePermissionAsync([FromServices] IPermissionService permissionService, [FromBody] Permission permission)
        {
            return await permissionService.UpdatePermissionAsync(permission);
        }
        private static async Task<Result> DeletePermissionAsync([FromServices] IPermissionService permissionService, Guid id)
        {
            return await permissionService.DeletePermissionAsync(id);
        }

    }
}