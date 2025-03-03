using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using VolunteerMgt.Server.Entities;

namespace VolunteerMgt.Server.Models.Edit
{
    public class EditVolunteerModel : VolunteerModel
    {
        [Required]
        [SwaggerSchema("Volunteer Id")]
        public string Id { get; set; } = string.Empty;

    }
}
