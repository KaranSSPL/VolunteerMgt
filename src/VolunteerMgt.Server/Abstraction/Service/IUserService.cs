using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.PasswordModel;
using VolunteerMgt.Server.Models.User;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IUserService : IScopedService
    {
        Task<Result<List<UserWithId>>> GetUsersAsync();
        Task<Result<UserWithId>> GetUserByIdAsync(Guid Id);
        Task<Result<EditUserModel>> UpdateUserAsync(EditUserModel model);
        Task<Result> ChangePasswordAsync(ChangePasswordModel model);
        Task<Result<RegisterUserModel>> AddUserAsync(RegisterUserModel model);
        Task<Result<UserRoleMapping>> GetUserRolesAsync(Guid Id);
        Task<Result> AssignRoleAsync(UserRoleMapping userRole);
        Task<Result> RemoveRoleAsync(UserRoleMapping userRole);
        Task<Result> DeleteUserAsync(string Id);
    }
}
