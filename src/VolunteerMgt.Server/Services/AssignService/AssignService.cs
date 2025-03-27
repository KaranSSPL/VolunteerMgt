using System;
using VolunteerMgt.Server.Abstraction.AssignService;
using VolunteerMgt.Server.Models.VolunteerService;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace VolunteerMgt.Server.Services.AssignService
{
    public class AssignService : IAssignService
    {
        private readonly DatabaseContext _context;

        public AssignService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignServiceToVolunteer(AssignRequest request)
        {
            var volunteer = await _context.Volunteer.FindAsync(request.VolunteerId);
            var service = await _context.Service.FindAsync(request.ServiceId);

            if (volunteer == null || service == null)
            {
                throw new ArgumentException("Invalid Volunteer ID or Service ID.");
            }


            var existingMapping = await _context.VolunteerServiceMapping
                .FirstOrDefaultAsync(vs => vs.VolunteerId == request.VolunteerId && vs.ServiceId == request.ServiceId);

            if (existingMapping != null)
            {
                existingMapping.ExitTime = request.ExitTime;
            }
            else
            {
                var mapping = new VolunteerServiceMapping
                {
                    VolunteerId = request.VolunteerId,
                    VolunteerName = volunteer.Name,
                    ServiceId = request.ServiceId,
                    ServiceName = service.ServiceName,
                    TimeSlot = request.TimeSlot,
                    ExitTime = request.ExitTime
                };

                _context.VolunteerServiceMapping.Add(mapping);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VolunteerServiceMapping>> GetVolunteerServices(int volunteerId)
        {
            return await _context.VolunteerServiceMapping
                .Where(vs => vs.VolunteerId == volunteerId)
                .ToListAsync();
        }

        public async Task<List<VolunteerServiceMapping>> GetAllVolunteerServiceMappings()
        {
            return await _context.VolunteerServiceMapping.ToListAsync();
        }

        public async Task<VolunteerServiceMapping?> GetVolunteerServiceMappingById(int id)
        {
            return await _context.VolunteerServiceMapping.FindAsync(id);
        }

        public async Task<bool> RemoveVolunteerService(int volunteerId, int serviceId)
        {
            var mapping = await _context.VolunteerServiceMapping
                .FirstOrDefaultAsync(vs => vs.VolunteerId == volunteerId && vs.ServiceId == serviceId);

            if (mapping != null)
            {
                _context.VolunteerServiceMapping.Remove(mapping);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteVolunteerWithServices(int volunteerId)
        {
            var volunteer = await _context.Volunteer.FindAsync(volunteerId);
            if (volunteer == null) return false;

            var mappings = await _context.VolunteerServiceMapping
                .Where(vs => vs.VolunteerId == volunteerId)
                .ToListAsync();

            _context.VolunteerServiceMapping.RemoveRange(mappings);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ServiceVolunteerCountDto>> GetServiceVolunteerCountsAsync()
        {
            var result = await _context.VolunteerServiceMapping
                .Where(vsm => string.IsNullOrEmpty(vsm.ExitTime)) 
                .GroupBy(vsm => new { vsm.ServiceId, vsm.ServiceName })
                .Select(group => new
                {
                    ServiceId = group.Key.ServiceId,
                    ServiceName = group.Key.ServiceName,
                    VolunteerCount = group.Count(),
                    RequiredVolunteer = _context.Service
                        .Where(s => s.Id == group.Key.ServiceId)
                        .Select(s => s.RequiredVolunteer)
                        .FirstOrDefault() ?? "0"
                })
                .ToListAsync();

            return result.Select(res => new ServiceVolunteerCountDto
            {
                ServiceId = res.ServiceId,
                ServiceName = res.ServiceName,
                VolunteerCount = res.VolunteerCount,
                RequiredVolunteer = res.RequiredVolunteer,
                PendingVolunteer = Math.Max((int.TryParse(res.RequiredVolunteer, out int reqVol) ? reqVol : 0) - res.VolunteerCount, 0)
            }).ToList();
        }

    }
}

