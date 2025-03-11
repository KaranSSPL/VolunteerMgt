using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Models.VolunteerService
{
    public class ServiceModel
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        public string RequiredVolunteer { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual List<VolunteerServiceMapping> VolunteerMappings { get; set; } = [];
    }
}
