using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Entities
{
    public class Coupons
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CouponValue { get; set; }

        [JsonIgnore]
        public  List<AdditionalCoupon>? AdditionalCoupons { get; set; }
    }
}
