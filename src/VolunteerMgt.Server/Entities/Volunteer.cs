using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Entities
{
    public class Volunteer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string MobileNo { get; set; } = string.Empty;
        public string? Address { get; set; }

        public string? Occupation { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? Image { get; set; }  

        public string? code { get; set; }

        public string? VolunteerType { get; set; }

        public virtual List<Availability> Availabilities { get; set; } = new();
    }
}
