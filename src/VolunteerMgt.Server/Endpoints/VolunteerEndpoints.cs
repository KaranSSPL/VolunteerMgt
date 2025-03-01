using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service.Volunteer;
using VolunteerMgt.Server.Models.Volunteers;

public static class VolunteerEndpoints
{
    public static void MapVolunteerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/volunteers")
            .WithOpenApi();
        
        group.MapPost("/add", AddVolunteersAsync)
            .WithName("AddVolunteers");

        group.MapGet("/all", GetAllVolunteersAsync)
            .WithName("GetAllVolunteers");

        group.MapGet("/{id}", GetVolunteerByIdAsync)
    .WithName("GetVolunteerById");

        group.MapPut("/update/{id}", UpdateVolunteerAsync)
            .WithName("UpdateVolunteer");

        group.MapDelete("/delete/{id}", DeleteVolunteerAsync)
            .WithName("DeleteVolunteer");
    }

    private static async Task<IResult> AddVolunteersAsync(
        [FromServices] IVolunteerService volunteerService,
        [FromBody] VolunteerModel volunteer)
    {
        var result = await volunteerService.AddVolunteerAsync(volunteer);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAllVolunteersAsync(
        [FromServices] IVolunteerService volunteerService)
    {
        var result = await volunteerService.GetAllVolunteersAsync();
        return Results.Ok(result);
    }
    private static async Task<IResult> GetVolunteerByIdAsync(
    [FromServices] IVolunteerService volunteerService,
    [FromRoute] int id)
    {
        var result = await volunteerService.GetVolunteerByIdAsync(id);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> UpdateVolunteerAsync(
        [FromServices] IVolunteerService volunteerService,
        [FromRoute] int id,
        [FromBody] VolunteerModel volunteer)
    {
        var result = await volunteerService.UpdateVolunteerAsync(id, volunteer);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteVolunteerAsync(
        [FromServices] IVolunteerService volunteerService,
        [FromRoute] int id)
    {
        var result = await volunteerService.DeleteVolunteerAsync(id);
        return Results.Ok(result);
    }
}
