using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service.VService;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Endpoints
{
    public static class ServiceEndpoints
    {
        public static void MapServiceEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/services")
                .WithOpenApi();

            // Create a new service
            group.MapPost("/add", AddServiceAsync)
                .WithName("AddService");

            // Get all services
            group.MapGet("/allService", GetAllServicesAsync)
                .WithName("GetAllServices");

            // Get service by ID
            group.MapGet("/{id}", GetServiceByIdAsync)
                .WithName("GetServiceById");

            // Update service name
            group.MapPut("/update/{id}", EditServiceNameAsync)
                .WithName("EditServiceName");

            // Delete service
            group.MapDelete("/delete/{id}", DeleteServiceAsync)
                .WithName("DeleteService");
        }

        private static async Task<IResult> AddServiceAsync(
            [FromServices] IVService serviceService,
            [FromBody] ServiceModel service)
        {
            var result = await serviceService.CreateServiceAsync(service);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetAllServicesAsync(
            [FromServices] IVService serviceService)
        {
            var result = await serviceService.GetAllServicesAsync();
            return Results.Ok(result);
        }

        private static async Task<IResult> GetServiceByIdAsync(
            [FromServices] IVService serviceService,
            [FromRoute] int id)
        {
            var serviceModel = await serviceService.GetServiceByIdAsync(id);
            return serviceModel is not null ? Results.Ok(serviceModel) : Results.NotFound();
        }

        private static async Task<IResult> EditServiceNameAsync(
            [FromServices] IVService serviceService,
            [FromRoute] int id,
            [FromBody] ServiceModel serviceModel)
        {
            var result = await serviceService.UpdateServiceNameAsync(id, serviceModel);
            return result ? Results.Ok("Service name updated successfully") : Results.NotFound("Service not found");
        }

        private static async Task<IResult> DeleteServiceAsync(
            [FromServices] IVService serviceService,
            [FromRoute] int id)
        {
            var result = await serviceService.DeleteServiceAsync(id);
            return result ? Results.Ok("Service deleted successfully") : Results.NotFound("Service not found");
        }
    }
}
