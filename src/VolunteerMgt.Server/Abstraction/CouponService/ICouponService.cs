using VolunteerMgt.Server.Abstraction.Service.Common;
using VolunteerMgt.Server.Models.Coupons;
using VolunteerMgt.Server.Persistence.Migrations;

namespace VolunteerMgt.Server.Abstraction.CouponService
{
    public interface ICouponService : IScopedService
    {
        Task<List<Coupons>> GetAllCouponsAsync();
        Task<Coupons?> GetCouponByIdAsync(int id);
        Task<Coupons> AddCouponAsync(Coupons coupon);
        Task<AdditionalCoupon?> AddAdditionalCouponAsync(int couponId, AdditionalCoupon additionalCoupon);
        Task<List<AdditionalCoupon>> GetAllAdditionalCouponsAsync();
    }
}
