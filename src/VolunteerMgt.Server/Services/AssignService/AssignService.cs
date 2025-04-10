using VolunteerMgt.Server.Abstraction.AssignService;
using VolunteerMgt.Server.Models.VolunteerService;
using VolunteerMgt.Server.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VolunteerMgt.Server.Models;

namespace VolunteerMgt.Server.Services.AssignService
{
    public class AssignService : IAssignService
    {
        private readonly DatabaseContext _context;

        public AssignService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Response<VolunteerServiceMapping>> AssignServiceToVolunteer(AssignRequest request)
        {
            var response = new Response<VolunteerServiceMapping>();
            try
            {
                var volunteer = await _context.Volunteer.FindAsync(request.VolunteerId);
                var service = await _context.Service.FindAsync(request.ServiceId); 
                VolunteerServiceMapping? mapping = await _context.VolunteerServiceMapping
                    .FirstOrDefaultAsync(vs => vs.VolunteerId == request.VolunteerId && vs.ServiceId == request.ServiceId);

                if (mapping != null)
                {
                    mapping.ExitTime = request.ExitTime;
                }
                else
                {
                    mapping = new VolunteerServiceMapping
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

                response.Success = true;
                response.Message = "Service assigned successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.Data = mapping;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        public async Task<Response<List<VolunteerServiceMapping>>> GetVolunteerServices(int volunteerId)
        {
            var response = new Response<List<VolunteerServiceMapping>>();
            try
            {
                var services = await _context.VolunteerServiceMapping
                    .Where(vs => vs.VolunteerId == volunteerId)
                    .ToListAsync();

                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Data = services;
                response.Message = services.Any()
                    ? "Volunteer services fetched successfully."
                    : "No services found for this volunteer.";

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error fetching services: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Data = new List<VolunteerServiceMapping>();
                return response;
            }
        }

        public async Task<List<VolunteerServiceMappingDto>> GetAllVolunteerServiceMappings()
        {
            try
            {
                var mappings = await _context.VolunteerServiceMapping
                    .Include(v => v.Volunteer)
                    .Include(s => s.Service)
                    .ToListAsync();

                var result = new List<VolunteerServiceMappingDto>();
                bool isUpdated = false;
                foreach (var mapping in mappings)
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
                        isUpdated = true; 
                    }
                    result.Add(new VolunteerServiceMappingDto
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
                    });
                }
                if (isUpdated)
                {
                    await _context.SaveChangesAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching volunteer service mappings: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<VolunteerServiceMappingDto>();
            }
        }

        public async Task<VolunteerServiceMapping?> GetVolunteerServiceMappingById(int id)
        {
            return await _context.VolunteerServiceMapping.FindAsync(id);
        }

        public async Task<Response<string>> RemoveVolunteerService(int volunteerId, int serviceId)
        {
            var response = new Response<string>();

            try
            {
                var mapping = await _context.VolunteerServiceMapping
                    .FirstOrDefaultAsync(vs => vs.VolunteerId == volunteerId && vs.ServiceId == serviceId);

                if (mapping == null)
                {
                    response.Success = false;
                    response.Message = "Mapping not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Data = null;
                    return response;
                }

                _context.VolunteerServiceMapping.Remove(mapping);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Mapping removed successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.Data = "Removed";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error removing mapping: {ex.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Data = null;
                return response;
            }
        }

        public async Task<List<ServiceVolunteerCountDto>> GetServiceVolunteerCountsAsync(string day)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var totalCouponsToday = await _context.VolunteerServiceMapping
                    .Where(vsm => vsm.TimeSlot.Date == today && string.IsNullOrEmpty(vsm.ExitTime))
                    .SumAsync(vsm => vsm.Coupon);
                var serviceData = await _context.Service
                    .Select(service => new
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        VolunteerCount = _context.VolunteerServiceMapping.Count(vsm => vsm.ServiceId == service.Id && string.IsNullOrEmpty(vsm.ExitTime)),
                        service.SaturdayVolunteerRequirement,
                        service.SundayVolunteerRequirement,
                        service.EkadashiVolunteerRequirement,
                        service.FestivalVolunteerRequirement
                    })
                    .ToListAsync(); 
                var result = serviceData.Select(service => new ServiceVolunteerCountDto
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName,
                    VolunteerCount = service.VolunteerCount,
                    RequiredVolunteer = day.ToLower() switch
                    {
                        "saturday" => service.SaturdayVolunteerRequirement,
                        "sunday" => service.SundayVolunteerRequirement,
                        "ekadashi" => service.EkadashiVolunteerRequirement,
                        "festival" => service.FestivalVolunteerRequirement,
                        _ => "0"
                    },
                    PendingVolunteer = Math.Max((int.TryParse(
                        day.ToLower() switch
                        {
                            "saturday" => service.SaturdayVolunteerRequirement,
                            "sunday" => service.SundayVolunteerRequirement,
                            "ekadashi" => service.EkadashiVolunteerRequirement,
                            "festival" => service.FestivalVolunteerRequirement,
                            _ => "0"
                        }, out int reqVol) ? reqVol : 0) - service.VolunteerCount, 0),
                    TotalCouponsToday = totalCouponsToday
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching service volunteer counts: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<ServiceVolunteerCountDto>();
            }
        }

        public async Task<bool> UpdateVolunteerServiceMappingAsync(VolunteerServiceMapping updatedMapping)
        {
            try
            {
                var existing = await _context.VolunteerServiceMapping.FindAsync(updatedMapping.Id);
                if (existing == null)
                    return false;
                var volunteer = await _context.Volunteer.FindAsync(updatedMapping.VolunteerId);
                var service = await _context.Service.FindAsync(updatedMapping.ServiceId);
                if (volunteer == null || service == null)
                    return false;
                existing.VolunteerId = updatedMapping.VolunteerId;
                existing.VolunteerName = volunteer.Name;
                existing.ServiceId = updatedMapping.ServiceId;
                existing.ServiceName = service.ServiceName;
                existing.TimeSlot = updatedMapping.TimeSlot;
                existing.ExitTime = updatedMapping.ExitTime;
                existing.BatchNumber = updatedMapping.BatchNumber;
                existing.Coupon = updatedMapping.Coupon;
                existing.TimeDifference = "";

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating volunteer service mapping: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }
    }
}

