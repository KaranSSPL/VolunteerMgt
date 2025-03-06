using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities.Identity;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IPermissionService : IScopedService
    {
        Task<Result<List<Permission>>> GetPermissionAsync();
        Task<Result<Permission>> GetPermissionByIdAsync(Guid id);
        Task<Result> AddPermissionAsync(Permission permission);
        Task<Result<Permission>> UpdatePermissionAsync(Permission permission);
        Task<Result> DeletePermissionAsync(Guid id);
    }
}
