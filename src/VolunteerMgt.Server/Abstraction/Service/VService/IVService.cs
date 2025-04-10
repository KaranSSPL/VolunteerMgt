using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Models;

namespace VolunteerMgt.Server.Abstraction.Service.VService
{
    public interface IVService : IScopedService
    {
        Task<Response<Entities.Service>> CreateServiceAsync(Entities.Service service);
        Task<List<Entities.Service>> GetAllServicesAsync();
        Task<Entities.Service?> GetServiceByIdAsync(int id);
        Task<Response<bool>> UpdateServiceAsync(int id, Entities.Service serviceModel);
        Task<Response<bool>> DeleteServiceAsync(int id);
    }
}
