using System.Text.Json.Serialization;
using VolunteerMgt.Server.Models.VolunteerService;

namespace VolunteerMgt.Server.Entities
{
    public class Service
    {
        public int Id { get; set; }

        public string ServiceName { get; set; } = string.Empty;

        public string SaturdayVolunteerRequirement { get; set; } = string.Empty;
        
        public string SundayVolunteerRequirement { get; set; } = string.Empty;
        
        public string EkadashiVolunteerRequirement { get; set; } = string.Empty;
        
        public string FestivalVolunteerRequirement { get; set; } = string.Empty;

        public DateTime DefaultTime { get; set; } 

        public int Code { get; set; }

        [JsonIgnore]
        public virtual List<VolunteerServiceMapping> VolunteerMappings { get; set; } = [];
    }
}
