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
                .WithOpenApi();

            group.MapPost("/addRole", AddRoleAsync)
                .WithName("addRole")
                .RequireAuthorization();

            group.MapPost("/updateRole", UpdateRoleAsync)
                .WithName("updateRole")
                .RequireAuthorization();


            group.MapPost("/assignUserRole", AssignUserRoleAsync)
                .WithName("assignUserRole")
                .RequireAuthorization();

            group.MapGet("/deleteRole/{id}", DeleteRoleAsync)
                .WithName("deleteRole")
                .RequireAuthorization();
        }
        private static async Task<Result> AddRoleAsync([FromServices] IRoleService roleService, [FromBody] string role)
        {
            return await roleService.AddRoleAsync(role);
        }
        private static async Task<Result> UpdateRoleAsync([FromServices] IRoleService roleService, [FromBody] Role role)
        {
            return await roleService.UpdateRoleAsync(role);
        }
        private static async Task<Result> AssignUserRoleAsync([FromServices] IRoleService roleService, [FromBody] AssignUser role)
        {
            return await roleService.AssignUserRoleAsync(role);
        }
        private static async Task<Result> DeleteRoleAsync([FromServices] IRoleService roleService, [FromQuery] string Id)
        {
            return await roleService.DeleteRoleAsync(Id);
        }
    }
}
