using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VolunteerMgt.Server.Models.Coupons
{
    public class Coupons
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CouponValue { get; set; }

        [JsonIgnore]
        public  List<AdditionalCoupon> AdditionalCoupons { get; set; }
    }
}
