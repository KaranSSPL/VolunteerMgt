using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string Image { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public virtual List<AvailabilityModel> Availabilities { get; set; } = [];
    }
}
