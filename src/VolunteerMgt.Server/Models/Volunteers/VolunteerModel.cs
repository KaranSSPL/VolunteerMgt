using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Models.Volunteers
{
    public class VolunteerModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string MobileNo { get; set; } = string.Empty;
        public string? Address { get; set; }

        public string? Occupation { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }  

        public string? code { get; set; }

        public string? VolunteerType { get; set; }

        public virtual List<AvailabilityModel> Availabilities { get; set; } = new();
    }
}
