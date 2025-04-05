using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Models.Volunteers
{
    public class AvailabilityModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? VolunteerId { get; set; } 
        public string? Day { get; set; }
        public string? TimeSlot { get; set; }
        [JsonIgnore]
        public virtual VolunteerModel Volunteer { get; set; }
    }
}
