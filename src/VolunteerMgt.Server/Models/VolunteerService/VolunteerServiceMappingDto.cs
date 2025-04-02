namespace VolunteerMgt.Server.Models.VolunteerService
{
    public class VolunteerServiceMappingDto
    {
        public int Id { get; set; }
        public int VolunteerId { get; set; }
        public string VolunteerName { get; set; }
        public DateTime TimeSlot { get; set; }  
        public string ExitTime { get; set; }  
        public string TimeDifference { get; set; }
        public int BatchNumber { get; set; }
        public int Coupon { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
    }
}
