using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.Volunteer;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IVolunteerService : IScopedService
    {
        Task<Result<List<VolunteerWithId>>> GetVolunteersAsync();
        Task<Result<VolunteerWithId>> GetVolunteerByIdAsync(Guid Id);
        Task<Result<EditVolunteerModel>> UpdateVolunteerAsync(EditVolunteerModel model);
        Task<Result> ChangePasswordAsync(ChangePasswordModel model);
        Task<Result<RegisterVolunteerModel>> AddVolunteerAsync(RegisterVolunteerModel model);
        Task<Result<UserRoleMapping>> GetVolunteerRolesAsync(Guid Id);
        Task<Result> AssignRoleAsync(UserRoleMapping userRole);
        Task<Result> RemoveRoleAsync(UserRoleMapping userRole);
        Task<Result> DeleteVolunteerAsync(string Id);
    }
}
