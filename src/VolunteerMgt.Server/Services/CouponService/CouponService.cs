using Microsoft.EntityFrameworkCore;
using VolunteerMgt.Server.Abstraction.CouponService;
using VolunteerMgt.Server.Models.Coupons;
using VolunteerMgt.Server.Persistence;

namespace VolunteerMgt.Server.Services.CouponService
{
    public class CouponService : ICouponService
    {
        private readonly DatabaseContext _db;

        public CouponService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Coupons>> GetAllCouponsAsync()
        {
            return await _db.Coupons.Include(c => c.AdditionalCoupons).ToListAsync();
        }

        public async Task<Coupons?> GetCouponByIdAsync(int id)
        {
            return await _db.Coupons.Include(c => c.AdditionalCoupons).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coupons> AddCouponAsync(Coupons coupon)
        {
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            return coupon;
        }

        public async Task<List<AdditionalCoupon>> GetAllAdditionalCouponsAsync()
        {
            var additionalCoupons = await _db.AdditionalCoupons.ToListAsync();

            var totalValues = await _db.AdditionalCoupons
                                       .GroupBy(ac => ac.CouponId)
                                       .Select(g => new { CouponId = g.Key, TotalValue = g.Sum(ac => ac.AdditionalCouponValue) })
                                       .ToListAsync();

            additionalCoupons.ForEach(ac =>
            {
                ac.TotalValue = totalValues.FirstOrDefault(tv => tv.CouponId == ac.CouponId)?.TotalValue ?? 0;
            });

            return additionalCoupons;
        }

        public async Task<AdditionalCoupon?> AddAdditionalCouponAsync(int couponId, AdditionalCoupon additionalCoupon)
        {
            var coupon = await _db.Coupons.FindAsync(couponId);
            if (coupon == null) return null;   
                additionalCoupon.CouponId = couponId;
                _db.AdditionalCoupons.Add(additionalCoupon);
                await _db.SaveChangesAsync();
                return additionalCoupon;
        }
    }
}
