namespace VolunteerMgt.Server.Models.VolunteerService
{
    public class AssignRequest
    {
        public int VolunteerId { get; set; }
        public int ServiceId { get; set; }
        public string TimeSlot { get; set; } = string.Empty;

        public DateTime createdDate { get; set; }
    }
}
