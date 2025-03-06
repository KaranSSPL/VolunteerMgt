using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.PermissionRoles;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IPermissionService : IScopedService
    {
        Task<Result<List<Permission>>> GetPermissionAsync();
        Task<Result> AddPermissionAsync(Permission permission);
        Task<Result<List<Permission>>> GetPermissionRolesAsync(Guid roleId);
        Task<Result<Permission>> UpdatePermissionAsync(Permission permission);
        Task<Result> DeletePermissionAsync(Guid id);
        Task<Result> AssignPermissionRoleAsync(PermissionRolesModel permissionRoles);
        Task<Result> RemovePermissionRoleAsync(PermissionRolesModel permissionRoles);
    }
}
