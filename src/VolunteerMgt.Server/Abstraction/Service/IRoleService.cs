using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IRoleService : IScopedService
    {
        Task<Result<List<Role>>> GetRolesAsync();
        Task<Result> AddRoleAsync(string role);
        Task<Result> DeleteRoleAsync(Guid roleId);
        Task<Result<Role>> UpdateRoleAsync(Role Role);
        Task<Result<Role>> GetRolesbyIdAsync(Guid roleId);
        Task<Result> AssignPermissionAsync(PermissionRolesDto permissionRoles);
        Task<Result> RemovePermissionAsync(PermissionRolesDto permissionRoles);

    }
}
