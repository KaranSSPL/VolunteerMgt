using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.Service.Volunteer;
using VolunteerMgt.Server.Models.Volunteers;
using VolunteerMgt.Server.Persistence;
using System.Net;
using VolunteerMgt.Server.Models;

namespace VolunteerMgt.Server.Services.Volunteer
{
    public class VolunteerService : IVolunteerService
    {
        private readonly DatabaseContext _dbContext;

        public VolunteerService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResponseModel<VolunteerModel>> AddVolunteerAsync(VolunteerModel request)
        {
            try
            {
                var volunteer = new VolunteerModel
                {
                    Name = request.Name,
                    MobileNo = request.MobileNo,
                    Address = request.Address,
                    Occupation = request.Occupation,
                    Availabilities = request.Availabilities.Select(a => new AvailabilityModel
                    {
                        Day = a.Day,
                        TimeSlot = a.TimeSlot
                    }).ToList()
                };

                _dbContext.Volunteer.Add(volunteer);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel<VolunteerModel>
                {
                    Success = true,
                    Message = "Volunteer added successfully!",
                    StatusCode = HttpStatusCode.Created,
                    Data = volunteer
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<VolunteerModel>
                {
                    Success = false,
                    Message = "Error adding volunteer: " + ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = null
                };
            }
        }

        public async Task<List<VolunteerModel>> GetAllVolunteersAsync()
        {
            return await _dbContext.Volunteer
                .Include(v => v.Availabilities)
                .ToListAsync();
        }

        public async Task<ResponseModel<VolunteerModel>> GetVolunteerByIdAsync(int id)
        {
            try
            {
                var volunteer = await _dbContext.Volunteer
                    .Include(v => v.Availabilities)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (volunteer == null)
                {
                    return new ResponseModel<VolunteerModel>
                    {
                        Success = false,
                        Message = "Volunteer not found!",
                        StatusCode = HttpStatusCode.NotFound,
                        Data = null
                    };
                }

                return new ResponseModel<VolunteerModel>
                {
                    Success = true,
                    Message = "Volunteer retrieved successfully!",
                    StatusCode = HttpStatusCode.OK,
                    Data = volunteer
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<VolunteerModel>
                {
                    Success = false,
                    Message = "Error retrieving volunteer: " + ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = null
                };
            }
        }

        public async Task<ResponseModel<VolunteerModel>> UpdateVolunteerAsync(int id, VolunteerModel request)
        {
            try
            {
                // Check if ID exists
                var volunteer = await _dbContext.Volunteer
                    .Include(v => v.Availabilities)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (volunteer == null)
                {
                    return new ResponseModel<VolunteerModel>
                    {
                        Success = false,
                        Message = "Volunteer not found!",
                        StatusCode = HttpStatusCode.NotFound,
                        Data = null
                    };
                }

                if (!string.IsNullOrEmpty(request.Name)) volunteer.Name = request.Name;
                if (!string.IsNullOrEmpty(request.MobileNo)) volunteer.MobileNo = request.MobileNo;
                if (!string.IsNullOrEmpty(request.Address)) volunteer.Address = request.Address;
                if (!string.IsNullOrEmpty(request.Occupation)) volunteer.Occupation = request.Occupation;

                if (request.Availabilities != null && request.Availabilities.Any())
                {
                    _dbContext.Availability.RemoveRange(volunteer.Availabilities);
                    volunteer.Availabilities = request.Availabilities.Select(a => new AvailabilityModel
                    {
                        Day = a.Day,
                        TimeSlot = a.TimeSlot
                    }).ToList();
                }

                _dbContext.Volunteer.Update(volunteer);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel<VolunteerModel>
                {
                    Success = true,
                    Message = "Volunteer updated successfully!",
                    StatusCode = HttpStatusCode.OK,
                    Data = volunteer
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<VolunteerModel>
                {
                    Success = false,
                    Message = "Error updating volunteer: " + ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = null
                };
            }
        }


        public async Task<ResponseModel<bool>> DeleteVolunteerAsync(int id)
        {
            try
            {
                var volunteer = await _dbContext.Volunteer.FindAsync(id);

                if (volunteer == null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = false,
                        Message = "Volunteer not found!",
                        StatusCode = HttpStatusCode.NotFound,
                        Data = false
                    };
                }

                _dbContext.Volunteer.Remove(volunteer);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Success = true,
                    Message = "Volunteer deleted successfully!",
                    StatusCode = HttpStatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Error deleting volunteer: " + ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = false
                };
            }
        }
    }

}
