using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.DataModals;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Models;

namespace VolunteerMgt.Server.Abstraction.Service.Volunteer
{
    public interface IVolunteerService : IScopedService
    {
        Task<ResponseModel<Entities.Volunteer>> AddVolunteerAsync(AddVolunteerDto request);
        //Task<string> SaveImageAsync(IFormFile imageFile);
        Task<List<Entities.Volunteer>> GetAllVolunteersAsync();
        Task<ResponseModel<Entities.Volunteer>> UpdateVolunteerAsync(int id, AddVolunteerDto request);
        Task<ResponseModel<bool>> DeleteVolunteerAsync(int id);
        Task<ResponseModel<Entities.Volunteer>> GetVolunteerByIdAsync(int id);

    }
}
