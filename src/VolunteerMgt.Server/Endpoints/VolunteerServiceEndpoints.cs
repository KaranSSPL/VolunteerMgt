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

            group.MapGet("/service-volunteer-counts", GetServiceVolunteerCountsAsync)
                .WithName("get-service-volunteer-counts");

            group.MapPut("/volunteer-service-mappings/{id}", UpdateVolunteerServiceMappingAsync)
                .WithName("update-volunteer-service-mapping");
        }

        private static async Task<IResult> AssignServiceAsync(
        [FromServices] IAssignService assignService,
        [FromBody] AssignRequest request)
        {
            var result = await assignService.AssignServiceToVolunteer(request);
            if (result.Success)
            {
                return Results.Ok(result);
            }
            return Results.Json(result, statusCode: (int)result.StatusCode);
        }

        private static async Task<IResult> GetVolunteerServicesAsync(
        [FromServices] IAssignService assignService,
        int id)
        {
            var result = await assignService.GetVolunteerServices(id);
            if (result.Success && result.Data != null && result.Data.Any())
            {
                return Results.Ok(result);
            }
            return Results.Json(result, statusCode: (int)result.StatusCode);
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
            if (result.Success)
            {
                return Results.Ok(result);
            }
            return Results.Json(result, statusCode: (int)result.StatusCode);
        }

        private static async Task<IResult> GetServiceVolunteerCountsAsync(
             [FromServices] IAssignService assignService,
             [FromQuery] string day)  
        {
            var counts = await assignService.GetServiceVolunteerCountsAsync(day); 
            return counts.Any() ? Results.Ok(counts) : Results.NotFound("No data found.");
        }

        private static async Task<IResult> UpdateVolunteerServiceMappingAsync(
            [FromServices] IAssignService assignService,
            [FromRoute] int id,
            [FromBody] VolunteerServiceMapping mapping)
        {
            if (id != mapping.Id)
                return Results.BadRequest("Mapping ID mismatch.");

            var result = await assignService.UpdateVolunteerServiceMappingAsync(mapping);
            return result ? Results.Ok("Volunteer service mapping updated successfully.") : Results.NotFound("Mapping not found or invalid data.");
        }
    }
}