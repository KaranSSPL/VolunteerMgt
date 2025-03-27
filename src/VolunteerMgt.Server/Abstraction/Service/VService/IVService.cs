using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Abstraction.Service.VService
{
    public interface IVService : IScopedService
    {
        Task<ServiceModel> CreateServiceAsync(ServiceModel service);
        Task<List<ServiceModel>> GetAllServicesAsync();
        Task<ServiceModel?> GetServiceByIdAsync(int id);
        Task<bool> UpdateServiceNameAsync(int id, ServiceModel serviceModel);
        Task<bool> DeleteServiceAsync(int id);
    }
}
