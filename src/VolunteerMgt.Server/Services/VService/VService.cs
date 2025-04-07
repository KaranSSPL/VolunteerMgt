using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.Service.VService;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services.VService
{
    public class VService : IVService
    {
        private readonly DatabaseContext _dbContext;

        public VService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            _dbContext.Service.Add(service);
            await _dbContext.SaveChangesAsync();
            return service;
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _dbContext.Service.ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _dbContext.Service.FindAsync(id);
        }

        public async Task<bool> UpdateServiceAsync(int id, Service serviceModel)
        {
            var existingService = await _dbContext.Service.FindAsync(id);
            if (existingService == null) return false;

            existingService.ServiceName = serviceModel.ServiceName;
            existingService.Code = serviceModel.Code;
            existingService.SaturdayVolunteerRequirement = serviceModel.SaturdayVolunteerRequirement;
            existingService.SundayVolunteerRequirement = serviceModel.SundayVolunteerRequirement;
            existingService.EkadashiVolunteerRequirement = serviceModel.EkadashiVolunteerRequirement;
            existingService.FestivalVolunteerRequirement = serviceModel.FestivalVolunteerRequirement;
            existingService.DefaultTime = serviceModel.DefaultTime;

            _dbContext.Entry(existingService).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _dbContext.Service.FindAsync(id);
            if (service == null) return false;

            _dbContext.Service.Remove(service);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
