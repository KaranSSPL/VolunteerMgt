using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Models.ToDo;

namespace VolunteerMgt.Server.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/todos")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetToDo)
            .WithName("getTodo");

        group.MapPost("/", PostTodo)
            .WithName("postTodo");
    }

    private static string GetToDo([FromServices] ITodoService todoService)
    {
        return "todoService.FetchHello()";
    }

    private static string PostTodo([FromServices] ITodoService todoService, [FromBody] PostTodoRequest request)
    {
        return "todoService.SendHello(request.Name)";
    }
}
