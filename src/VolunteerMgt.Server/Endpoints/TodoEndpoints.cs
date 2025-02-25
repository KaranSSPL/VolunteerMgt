using Microsoft.AspNetCore.Mvc;

namespace VolunteerMgt.Server.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        //app.MapGet("/api/todos", GetToDo)
        //    .RequireAuthorization()
        //    .WithOpenApi()
        //    .WithName("getTodo");

        //app.MapPost("/api/todos", PostTodo)
        //    .RequireAuthorization()
        //    .WithOpenApi()
        //    .WithName("postTodo");

        var group = app.MapGroup("/api/todos")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetToDo)
            .WithName("getTodo");

        group.MapPost("/", PostTodo)
            .WithName("postTodo");
    }

    private static string GetToDo(ITodoService todoService)
    {
        return "helloService.FetchHello()";
    }

    private static string PostTodo(ITodoService todoService, [FromBody] PostTodoRequest request)
    {
        return "helloService.SendHello(request.Name)";
    }
}
