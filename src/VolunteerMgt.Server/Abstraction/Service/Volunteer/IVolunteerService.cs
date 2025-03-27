using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.DataModals;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Models.Volunteers;

namespace VolunteerMgt.Server.Abstraction.Service.Volunteer
{
    public interface IVolunteerService : IScopedService
    {
        Task<ResponseModel<VolunteerModel>> AddVolunteerAsync(AddVolunteerDto request);
        //Task<string> SaveImageAsync(IFormFile imageFile);
        Task<List<VolunteerModel>> GetAllVolunteersAsync();
        Task<ResponseModel<VolunteerModel>> UpdateVolunteerAsync(int id, AddVolunteerDto request);
        Task<ResponseModel<bool>> DeleteVolunteerAsync(int id);
        Task<ResponseModel<VolunteerModel>> GetVolunteerByIdAsync(int id);

    }
}
