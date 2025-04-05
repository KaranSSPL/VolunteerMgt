using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Abstraction.AssignService
{
    public interface IAssignService : IScopedService
    {
        Task<bool> AssignServiceToVolunteer(AssignRequest requests);
        Task<List<VolunteerServiceMapping>> GetVolunteerServices(int volunteerId);
        Task<List<VolunteerServiceMappingDto>> GetAllVolunteerServiceMappings();
        Task<VolunteerServiceMapping?> GetVolunteerServiceMappingById(int id);
        Task<bool> RemoveVolunteerService(int volunteerId, int serviceId);  
        Task<List<ServiceVolunteerCountDto>> GetServiceVolunteerCountsAsync(string day);
        Task<bool> UpdateVolunteerServiceMappingAsync(VolunteerServiceMapping updatedMapping);
    }
}
