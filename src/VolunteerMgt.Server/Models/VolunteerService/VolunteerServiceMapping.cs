using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VolunteerMgt.Server.Models.Volunteers;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Models
{
    public class VolunteerServiceMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int VolunteerId { get; set; }

        [Required]
        public string VolunteerName { get; set; }

        [ForeignKey("VolunteerId")]
        public VolunteerModel Volunteer { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Required]
        public string TimeSlot { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("ServiceId")]
        public ServiceModel Service { get; set; }
    }
}
