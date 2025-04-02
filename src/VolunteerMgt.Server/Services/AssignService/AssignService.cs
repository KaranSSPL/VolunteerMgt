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
            try
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
                        BatchNumber = request.BatchNumber,
                        ExitTime = request.ExitTime,
                        Coupon = request.Coupon,
                        TimeDifference = ""
                    };
                    _context.VolunteerServiceMapping.Add(mapping);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                return true;
        }

        public async Task<List<VolunteerServiceMapping>> GetVolunteerServices(int volunteerId)
        {
            return await _context.VolunteerServiceMapping
                .Where(vs => vs.VolunteerId == volunteerId)
                .ToListAsync();
        }

        public async Task<List<VolunteerServiceMappingDto>> GetAllVolunteerServiceMappings()
        {
            var mappings = await _context.VolunteerServiceMapping
                .Include(v => v.Volunteer)
                .Include(s => s.Service)
                .ToListAsync();
            var result = mappings.Select(mapping =>
            {
                DateTime exitTimeParsed;
                DateTime timeSlotParsed = mapping.TimeSlot;
                TimeSpan exitTime = TimeSpan.Zero;
                if (DateTime.TryParse(mapping.ExitTime, out exitTimeParsed))
                {
                    exitTime = exitTimeParsed.TimeOfDay; 
                }
                string formattedExitTime = exitTime != TimeSpan.Zero
                    ? exitTimeParsed.ToString("hh:mm tt")
                    : "";
                string timeDifferenceString = mapping.TimeDifference;
                if (string.IsNullOrEmpty(timeDifferenceString) && exitTime != TimeSpan.Zero)
                {
                    TimeSpan timeSlotTime = timeSlotParsed.TimeOfDay;

                    if (exitTime < timeSlotTime)
                    {
                        exitTime = exitTime.Add(new TimeSpan(24, 0, 0));
                    }
                    TimeSpan timeDifference = exitTime - timeSlotTime;
                    timeDifferenceString = $"{(int)timeDifference.TotalHours} hours {(int)timeDifference.Minutes} minutes";
                    mapping.TimeDifference = timeDifferenceString;
                    _context.VolunteerServiceMapping.Update(mapping);
                }
                return new VolunteerServiceMappingDto
                {
                    Id = mapping.Id,
                    VolunteerId = mapping.VolunteerId,
                    VolunteerName = mapping.VolunteerName,
                    TimeSlot = mapping.TimeSlot,
                    ExitTime = formattedExitTime,
                    TimeDifference = timeDifferenceString,
                    BatchNumber = mapping.BatchNumber,
                    Coupon = mapping.Coupon,
                    ServiceName = mapping.ServiceName,
                    ServiceId = mapping.ServiceId
                };
            }).ToList();
            await _context.SaveChangesAsync();
            return result;
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
            var today = DateTime.UtcNow.Date; 

            var totalCouponsToday = await _context.VolunteerServiceMapping
                .Where(vsm => vsm.TimeSlot.Date == today && string.IsNullOrEmpty(vsm.ExitTime))
                .SumAsync(vsm => vsm.Coupon);

            var result = await _context.Service
                .GroupJoin(
                    _context.VolunteerServiceMapping
                        .Where(vsm => string.IsNullOrEmpty(vsm.ExitTime)),
                    service => service.Id,
                    vsm => vsm.ServiceId,
                    (service, vsmGroup) => new
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        VolunteerCount = vsmGroup.Count(),
                        RequiredVolunteer = service.RequiredVolunteer ?? "0"
                    })
                .ToListAsync();

            return result.Select(res => new ServiceVolunteerCountDto
            {
                ServiceId = res.ServiceId,
                ServiceName = res.ServiceName,
                VolunteerCount = res.VolunteerCount,
                RequiredVolunteer = res.RequiredVolunteer,
                PendingVolunteer = Math.Max((int.TryParse(res.RequiredVolunteer, out int reqVol) ? reqVol : 0) - res.VolunteerCount, 0),
                TotalCouponsToday = totalCouponsToday 
            }).ToList();
        }

    }
}

