using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.AssignService;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Endpoints
{
    public static class VolunteerServiceEndpoints
    {
        public static void MapVolunteerServiceEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/volunteer-service")
                .WithOpenApi();

            group.MapPost("/assign", AssignServiceAsync)
                .WithName("assign-service");

            group.MapGet("/volunteer/{id}/services", GetVolunteerServicesAsync)
                .WithName("get-volunteer-services");

            group.MapGet("/volunteer-service-mappings", GetAllVolunteerServiceMappingsAsync)
                .WithName("get-all-volunteer-service-mappings");

            group.MapGet("/volunteer-service-mappings/{id}", GetVolunteerServiceMappingByIdAsync)
                .WithName("get-volunteer-service-mapping-by-id");

            group.MapDelete("/volunteer/{volunteerId}/service/{serviceId}", RemoveVolunteerServiceAsync)
                .WithName("remove-volunteer-service");

            group.MapDelete("/volunteer/{volunteerId}", DeleteVolunteerWithServicesAsync)
    .WithName("delete-volunteer-with-services");

        }

        private static async Task<IResult> AssignServiceAsync(
            [FromServices] IAssignService assignService,
            [FromBody] List<AssignRequest> requests)
        {
            var result = await assignService.AssignServiceToVolunteer(requests);
            return result ? Results.Ok("Services assigned successfully.") : Results.BadRequest("Failed to assign services.");
        }

        private static async Task<IResult> GetVolunteerServicesAsync(
            [FromServices] IAssignService assignService,
            int id)
        {
            var services = await assignService.GetVolunteerServices(id);
            return services.Any() ? Results.Ok(services) : Results.NotFound("No services found for this volunteer.");
        }

        private static async Task<IResult> GetAllVolunteerServiceMappingsAsync(
            [FromServices] IAssignService assignService)
        {
            var mappings = await assignService.GetAllVolunteerServiceMappings();
            return mappings.Any() ? Results.Ok(mappings) : Results.NotFound("No volunteer service mappings found.");
        }

        private static async Task<IResult> GetVolunteerServiceMappingByIdAsync(
             [FromServices] IAssignService assignService,
             int id)
        {
            var mapping = await assignService.GetVolunteerServiceMappingById(id);
            if (mapping == null)
                return Results.NotFound("Volunteer service mapping not found.");

            var volunteerServices = await assignService.GetVolunteerServices(mapping.VolunteerId);

            return Results.Ok(new
            {
                VolunteerDetails = mapping,
                AssignedServices = volunteerServices
            });
        }

        private static async Task<IResult> RemoveVolunteerServiceAsync(
            [FromServices] IAssignService assignService,
            int volunteerId,
            int serviceId)
        {
            var result = await assignService.RemoveVolunteerService(volunteerId, serviceId);
            return result ? Results.Ok("Service removed successfully.") : Results.NotFound("Service not found for this volunteer.");
        }

        private static async Task<IResult> DeleteVolunteerWithServicesAsync(
    [FromServices] IAssignService assignService,
    int volunteerId)
        {
            var result = await assignService.DeleteVolunteerWithServices(volunteerId);
            return result ? Results.Ok("Volunteer and all assigned services deleted successfully.")
                          : Results.NotFound("Volunteer not found.");
        }

    }
}