using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.Service.Volunteer;
using VolunteerMgt.Server.Models.Volunteers;
using VolunteerMgt.Server.Persistence;
using System.Net;
using VolunteerMgt.Server.Models;
using Microsoft.AspNetCore.Hosting;
using VolunteerMgt.Server.DataModals;
using Newtonsoft.Json;

namespace VolunteerMgt.Server.Services.Volunteer
{
    public class VolunteerService : IVolunteerService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public VolunteerService(DatabaseContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseModel<VolunteerModel>> AddVolunteerAsync(AddVolunteerDto request)
        {
            try
            {
                string imagePath = await SaveImageAsync(request.Image);
                var volunteer = new VolunteerModel
                {
                    Name = request.Name,
                    MobileNo = request.MobileNo,
                    Address = request.Address,
                    Occupation = request.Occupation,
                    code = request.code,
                    ImagePath = imagePath
                };
                var availabilities = JsonConvert.DeserializeObject<List<AvailabilityDataModel>>(request.Availabilities);
                if (availabilities != null && availabilities.Any())
                {
                    volunteer.Availabilities = availabilities.Select(a => new AvailabilityModel
                    {
                        Day = a.Day,
                        TimeSlot = a.TimeSlot,
                    }).ToList();
                }
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

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }
            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(imageFile.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(imageFile.FileName)}";
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return $"/images/{uniqueFileName}";
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

        public async Task<ResponseModel<VolunteerModel>> UpdateVolunteerAsync(int id, AddVolunteerDto request)
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

                if (!string.IsNullOrEmpty(request.Name)) volunteer.Name = request.Name;
                if (!string.IsNullOrEmpty(request.MobileNo)) volunteer.MobileNo = request.MobileNo;
                if (!string.IsNullOrEmpty(request.Address)) volunteer.Address = request.Address;
                if (!string.IsNullOrEmpty(request.Occupation)) volunteer.Occupation = request.Occupation;
                if (!string.IsNullOrEmpty(request.code)) volunteer.code = request.code;

                if (request.Image != null && request.Image.Length > 0)
                {
                    volunteer.ImagePath = await SaveImageAsync(request.Image); 
                }
                else if (!string.IsNullOrEmpty(request.ImagePath))
                {
                    volunteer.ImagePath = request.ImagePath; 
                }

                var availabilities = JsonConvert.DeserializeObject<List<AvailabilityDataModel>>(request.Availabilities);
                if (availabilities != null && availabilities.Any())
                {
                    _dbContext.Availability.RemoveRange(volunteer.Availabilities);
                    volunteer.Availabilities = availabilities.Select(a => new AvailabilityModel
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
