using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.Role;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IRoleService : IScopedService
    {
        Task<Result<List<Role>>> GetRoleAsync();
        Task<Result> AddRoleAsync(string role);
        Task<Result> DeleteRoleAsync(Guid roleId);
        Task<Result<Role>> UpdateRoleAsync(Role Role);
        Task<Result<AssignUser>> GetUserRolesAsync(Guid Id);
        Task<Result> AssignUserRoleAsync(AssignUser user);
        Task<Result> RemoveUserRoleAsync(AssignUser assignUser);
    }
}
