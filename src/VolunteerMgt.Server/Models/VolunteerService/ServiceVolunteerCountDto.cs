namespace VolunteerMgt.Server.Models.VolunteerService
{
    public class ServiceVolunteerCountDto
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int VolunteerCount { get; set; }
        public string RequiredVolunteer { get; set; }
        public int PendingVolunteer { get; set; }

    }
}
