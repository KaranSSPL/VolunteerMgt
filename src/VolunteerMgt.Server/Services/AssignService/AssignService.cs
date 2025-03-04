using System;
using VolunteerMgt.Server.Abstraction.AssignService;
using VolunteerMgt.Server.Models.VolunteerService;
using VolunteerMgt.Server.Models;
using VolunteerMgt.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace VolunteerMgt.Server.Services.AssignService
{
    public class AssignService : IAssignService
    {
        private readonly DatabaseContext _context;

        public AssignService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignServiceToVolunteer(List<AssignRequest> requests)
        {
            foreach (var request in requests)
            {
                var volunteer = await _context.Volunteer.FindAsync(request.VolunteerId);
                var service = await _context.Service.FindAsync(request.ServiceId);
                if (volunteer != null && service != null)
                {
                    _context.VolunteerServiceMapping.Add(new VolunteerServiceMapping
                    {
                        VolunteerId = request.VolunteerId,
                        VolunteerName = volunteer.Name,
                        ServiceId = request.ServiceId,
                        ServiceName = service.ServiceName,
                        TimeSlot = request.TimeSlot,
                        CreatedDate = request.createdDate
                    });
                }
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

    }
}

