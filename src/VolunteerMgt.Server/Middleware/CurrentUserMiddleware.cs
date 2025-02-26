using VolunteerMgt.Server.Abstraction.Service.Identity;

namespace VolunteerMgt.Server.Middleware;

public sealed class CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        currentUserInitializer.SetCurrentUser(context.User);
        await next(context);
    }
}