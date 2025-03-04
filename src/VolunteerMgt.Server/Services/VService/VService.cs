using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.Service.VService;
using VolunteerMgt.Server.Models.VolunteerService;
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

        public async Task<ServiceModel> CreateServiceAsync(ServiceModel service)
        {
            _dbContext.Service.Add(service);
            await _dbContext.SaveChangesAsync();
            return service;
        }

        public async Task<List<ServiceModel>> GetAllServicesAsync()
        {
            return await _dbContext.Service.ToListAsync();
        }

        public async Task<ServiceModel?> GetServiceByIdAsync(int id)
        {
            return await _dbContext.Service.FindAsync(id);
        }

        public async Task<bool> UpdateServiceNameAsync(int id, ServiceModel serviceModel)
        {
            var existingService = await _dbContext.Service.FindAsync(id);
            if (existingService == null) return false;

            existingService.ServiceName = serviceModel.ServiceName;
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
