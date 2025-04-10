using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Abstraction.Service.VService;
using VolunteerMgt.Server.Entities;
using VolunteerMgt.Server.Models;
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

        public async Task<Response<Service>> CreateServiceAsync(Service service)
        {
            var response = new Response<Service>();
            try
            {
                _dbContext.Service.Add(service);
                await _dbContext.SaveChangesAsync();

                response.Success = true;
                response.Message = "Service created successfully.";
                response.StatusCode = HttpStatusCode.Created;
                response.Data = service;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while creating the service: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Data = null;
            }

            return response;
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _dbContext.Service.ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _dbContext.Service.FindAsync(id);
        }

        public async Task<Response<bool>> UpdateServiceAsync(int id, Service serviceModel)
        {
            var response = new Response<bool>();
            try
            {
                var existingService = await _dbContext.Service.FindAsync(id);
                if (existingService == null)
                {
                    response.Success = false;
                    response.Message = $"Service with ID {id} not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Data = false;
                    return response;
                }
                existingService.ServiceName = serviceModel.ServiceName;
                existingService.Code = serviceModel.Code;
                existingService.SaturdayVolunteerRequirement = serviceModel.SaturdayVolunteerRequirement;
                existingService.SundayVolunteerRequirement = serviceModel.SundayVolunteerRequirement;
                existingService.EkadashiVolunteerRequirement = serviceModel.EkadashiVolunteerRequirement;
                existingService.FestivalVolunteerRequirement = serviceModel.FestivalVolunteerRequirement;
                existingService.DefaultTime = serviceModel.DefaultTime;
                _dbContext.Entry(existingService).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                response.Success = true;
                response.Message = "Service updated successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while updating the service: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Data = false;
            }
            return response;
        }

        public async Task<Response<bool>> DeleteServiceAsync(int id)
        {
            var response = new Response<bool>();
            try
            {
                var service = await _dbContext.Service.FindAsync(id);
                if (service == null)
                {
                    response.Success = false;
                    response.Message = $"Service with ID {id} not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Data = false;
                    return response;
                }
                _dbContext.Service.Remove(service);
                await _dbContext.SaveChangesAsync();
                response.Success = true;
                response.Message = "Service deleted successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred while deleting the service: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Data = false;
            }
            return response;
        }
    }
}
