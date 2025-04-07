using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Entities
{
    public class AdditionalCoupon
    {
        public int Id { get; set; }
        public int AdditionalCouponValue { get; set; }

        public int CouponId { get; set; }

        public int TotalValue { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public  Coupons? Coupon { get; set; }
    }
}
