using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.DataModals;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Models;

namespace VolunteerMgt.Server.Abstraction.Service.Volunteer
{
    public interface IVolunteerService : IScopedService
    {
        Task<Response<Entities.Volunteer>> AddVolunteerAsync(AddVolunteerDto request);
        //Task<string> SaveImageAsync(IFormFile imageFile);
        Task<List<Entities.Volunteer>> GetAllVolunteersAsync();
        Task<Response<Entities.Volunteer>> UpdateVolunteerAsync(int id, AddVolunteerDto request);
        Task<Response<bool>> DeleteVolunteerAsync(int id);
        Task<Response<Entities.Volunteer>> GetVolunteerByIdAsync(int id);

    }
}
