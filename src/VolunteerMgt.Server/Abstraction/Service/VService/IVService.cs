using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Abstraction.Service.VService
{
    public interface IVService : IScopedService
    {
        Task<Entities.Service> CreateServiceAsync(Entities.Service service);
        Task<List<Entities.Service>> GetAllServicesAsync();
        Task<Entities.Service?> GetServiceByIdAsync(int id);
        Task<bool> UpdateServiceAsync(int id, Entities.Service serviceModel);
        Task<bool> DeleteServiceAsync(int id);
    }
}
