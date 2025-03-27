using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteerMgt.Server.Models.Volunteers;

namespace VolunteerMgt.Server.DataModals;

public class AddVolunteerDto
{
    public string Name { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;

    [NotMapped]
    public IFormFile Image { get; set; }
    public string code { get; set; } = string.Empty;
    [JsonProperty("availabilities")]
    public string Availabilities { get; set; } = string.Empty;
}

public class AvailabilityDataModel
{
    [JsonProperty("day")]
    public string Day { get; set; } = string.Empty;
    [JsonProperty("timeSlot")]
    public string TimeSlot { get; set; } = string.Empty;
}