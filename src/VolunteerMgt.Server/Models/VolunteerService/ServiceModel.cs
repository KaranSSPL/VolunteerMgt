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
        public string SaturdayVolunteerRequirement { get; set; } = string.Empty;
        
        [Required]
        public string SundayVolunteerRequirement { get; set; } = string.Empty;
        
        [Required]
        public string EkadashiVolunteerRequirement { get; set; } = string.Empty;
        
        [Required]
        public string FestivalVolunteerRequirement { get; set; } = string.Empty;

        [Required]
        public DateTime DefaultTime { get; set; } 

        [Required]
        public int Code { get; set; }

        [JsonIgnore]
        public virtual List<VolunteerServiceMapping> VolunteerMappings { get; set; } = [];
    }
}
