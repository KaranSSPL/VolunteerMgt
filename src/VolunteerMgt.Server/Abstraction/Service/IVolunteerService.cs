using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.ChangePassword;
using VolunteerMgt.Server.Models.Edit;
using VolunteerMgt.Server.Models.ForgotPassword;
using VolunteerMgt.Server.Models.ResetPassword;
using VolunteerMgt.Server.Models.Volunteer;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IVolunteerService : IScopedService
    {
        Task<Result<List<ApplicationUser>>> GetVolunteersAsync();
        Task<Result<VolunteerWithId>> GetVolunteerByIdAsync(string Id);
        Task<Result<ApplicationUser>> UpdateVolunteerAsync(EditVolunteerModel model);
        Task<Result> ForgotPasswordAsync(ForgotPasswordModel model);
        Task<Result> ResetPasswordAsync(string email, string token);
        Task<Result> ResetPasswordPostAsync(ResetPasswordModel model);
        Task<Result> ChangePasswordAsync(ChangePasswordModel model);
    }
}
