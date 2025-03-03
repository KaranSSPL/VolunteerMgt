using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.Register;
using VolunteerMgt.Server.Models.Wrapper;

namespace VolunteerMgt.Server.Abstraction.Service
{
    public interface IUserService : IScopedService
    {
        Task<Result<RegisterModel>> AddUserAsync(RegisterModel model);
        Task<Result> DeleteUserAsync(string Id);
    }
}
