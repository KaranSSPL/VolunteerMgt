namespace VolunteerMgt.Server.Models.VolunteerService
{
    public class AssignRequest
    {
        public int VolunteerId { get; set; }
        public int ServiceId { get; set; }
        public DateTime TimeSlot { get; set; }
        public string ExitTime { get; set; } = string.Empty;
    }
}
