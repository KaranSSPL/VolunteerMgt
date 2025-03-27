using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Models.Volunteers
{
    public class VolunteerModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        [NotMapped]
        
        public IFormFile Image { get; set; }
        public string code { get; set; } = string.Empty;
        public virtual List<AvailabilityModel> Availabilities { get; set; } = [];
    }
}
