using Microsoft.AspNetCore.Mvc;
using VolunteerMgt.Server.Abstraction.Service;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.ChangePassword;
using VolunteerMgt.Server.Models.Edit;
using VolunteerMgt.Server.Models.ForgotPassword;
using VolunteerMgt.Server.Models.ResetPassword;
using VolunteerMgt.Server.Models.Volunteer;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Endpoints
{
    public static class VolunteerEndpoints
    {
        public static void MapVolunteerEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/volunteer")
                .WithOpenApi()
                .RequireAuthorization();

            group.MapGet("/get-Volunteers", GetVolunteersAsync)
                .WithName("getAllVolunteers");

            group.MapGet("/get-Volunteers/{id}", GetVolunteerbyIdAsync)
                .WithName("getVolunteerById");

            group.MapPut("/update-volunteer", UpdateVolunteerAsync)
                .WithName("updateVolunteer");

            group.MapPost("/forgot-password", ForgotPasswordAsync)
                .WithName("forgotPassword")
                .AllowAnonymous();

            group.MapGet("/reset-password", ResetPasswordAsync)
                .WithName("resetPassword")
                .AllowAnonymous();

            group.MapPost("/reset-password", ResetPasswordPostAsync)
                .WithName("resetPasswordPost")
                .AllowAnonymous();

            group.MapPut("/change-password", ChangePasswordAsync)
                .WithName("changePassword");
        }
        private static async Task<Result<List<ApplicationUser>>> GetVolunteersAsync([FromServices] IVolunteerService volunteerService)
        {
            return await volunteerService.GetVolunteersAsync();
        }
        private static async Task<Result<VolunteerWithId>> GetVolunteerbyIdAsync([FromServices] IVolunteerService volunteerService, Guid Id)
        {
            return await volunteerService.GetVolunteerByIdAsync(Id);
        }
        private static async Task<Result<EditVolunteerModel>> UpdateVolunteerAsync([FromServices] IVolunteerService volunteerService, [FromBody] EditVolunteerModel model)
        {
            return await volunteerService.UpdateVolunteerAsync(model);
        }
        private static async Task<Result> ForgotPasswordAsync([FromServices] IVolunteerService volunteerService, [FromBody] ForgotPasswordModel model)
        {
            return await volunteerService.ForgotPasswordAsync(model);
        }
        private static async Task<Result> ResetPasswordAsync([FromServices] IVolunteerService volunteerService, [FromQuery] string email, [FromQuery] string token)
        {
            return await volunteerService.ResetPasswordAsync(email, token);
        }
        private static async Task<Result> ResetPasswordPostAsync([FromServices] IVolunteerService volunteerService, [FromBody] ResetPasswordModel model)
        {
            return await volunteerService.ResetPasswordPostAsync(model);
        }

        private static async Task<Result> ChangePasswordAsync([FromServices] IVolunteerService volunteerService, [FromBody] ChangePasswordModel model)
        {
            return await volunteerService.ChangePasswordAsync(model);
        }

    }
}
