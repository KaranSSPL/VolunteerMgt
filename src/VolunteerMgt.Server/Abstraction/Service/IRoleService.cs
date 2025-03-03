using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.Role;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IRoleService : IScopedService
    {
        Task<Result> AddRoleAsync(string role);
        Task<Result> DeleteRoleAsync(string roleId);
        Task<Result> UpdateRoleAsync(Role Role);
        Task<Result> AssignUserRoleAsync(AssignUser user);
    }
}
