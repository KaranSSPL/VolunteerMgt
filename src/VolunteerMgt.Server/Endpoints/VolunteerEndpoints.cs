using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.Volunteer;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class VolunteerEndpoints
    {
        public static void MapVolunteerEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/volunteers")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/", GetVolunteersAsync)
                .WithName("getVolunteers");

            group.MapGet("/{id}", GetVolunteerbyIdAsync)
                .WithName("getVolunteerById");

            group.MapGet("/{id}/roles", GetVolunteerRolesAsync)
                .WithName("getVolunteerRoles");

            group.MapPost("/", AddVolunteerAsync)
                .WithName("AddVolunteer")
                .AllowAnonymous();

            group.MapPut("/", UpdateVolunteerAsync)
                .WithName("updateVolunteer");

            group.MapDelete("/{id}", DeleteVolunteerAsync)
                .WithName("deleteVolunteer");

            group.MapPatch("/assign-role", AssignRoleAsync)
                .WithName("assignVolunteerRoles");

            group.MapPatch("/remove-role", RemoveRoleAsync)
                .WithName("removeVolunteerRoles");

            group.MapPut("/change-password", ChangePasswordAsync)
                .WithName("changePassword");
        }
        private static async Task<Result<List<VolunteerWithId>>> GetVolunteersAsync([FromServices] IVolunteerService volunteerService)
        {
            return await volunteerService.GetVolunteersAsync();
        }
        private static async Task<Result<VolunteerWithId>> GetVolunteerbyIdAsync([FromServices] IVolunteerService volunteerService, Guid id)
        {
            return await volunteerService.GetVolunteerByIdAsync(id);
        }
        private static async Task<Result<EditVolunteerModel>> UpdateVolunteerAsync([FromServices] IVolunteerService volunteerService, [FromBody] EditVolunteerModel model)
        {
            return await volunteerService.UpdateVolunteerAsync(model);
        }

        private static async Task<Result> ChangePasswordAsync([FromServices] IVolunteerService volunteerService, [FromBody] ChangePasswordModel model)
        {
            return await volunteerService.ChangePasswordAsync(model);
        }
        private static async Task<Result<RegisterVolunteerModel>> AddVolunteerAsync([FromServices] IVolunteerService volunteerService, [FromBody] RegisterVolunteerModel model)
        {
            return await volunteerService.AddVolunteerAsync(model);
        }
        private static async Task<Result> GetVolunteerRolesAsync([FromServices] IVolunteerService volunteerService, Guid id)
        {
            return await volunteerService.GetVolunteerRolesAsync(id);
        }
        private static async Task<Result> AssignRoleAsync([FromServices] IVolunteerService volunteerService, [FromBody] UserRoleMapping userRole)
        {
            return await volunteerService.AssignRoleAsync(userRole);
        }
        private static async Task<Result> RemoveRoleAsync([FromServices] IVolunteerService volunteerService, [FromBody] UserRoleMapping userRole)
        {
            return await volunteerService.RemoveRoleAsync(userRole);
        }
        private static async Task<Result> DeleteVolunteerAsync([FromServices] IVolunteerService volunteerService, string id)
        {
            return await volunteerService.DeleteVolunteerAsync(id);
        }
    }
}
