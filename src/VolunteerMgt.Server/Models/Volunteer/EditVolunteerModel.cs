using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VolunteerMgt.Server.Models.Volunteer
{
    public class EditVolunteerModel : Entities.Volunteer
    {
        [Required]
        [SwaggerSchema("Volunteer Id")]
        public string Id { get; set; } = string.Empty;

    }
}
