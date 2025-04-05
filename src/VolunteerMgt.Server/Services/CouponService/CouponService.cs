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
            try
            {
                return await _db.Coupons
                    .Include(c => c.AdditionalCoupons)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching coupon by ID: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }

        public async Task<Coupons> AddCouponAsync(Coupons coupon)
        {
            try
            {
                await _db.Coupons.AddAsync(coupon);
                await _db.SaveChangesAsync();
                return coupon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding coupon: {ex.Message}\n{ex.InnerException?.Message}");
                return null;
            }
        }

        public async Task<List<AdditionalCoupon>> GetAllAdditionalCouponsAsync()
        {
            try
            {
                var additionalCoupons = await _db.AdditionalCoupons.ToListAsync();

                if (!additionalCoupons.Any()) return additionalCoupons;

                var totalValuesDict = await _db.AdditionalCoupons
                    .GroupBy(ac => ac.CouponId)
                    .ToDictionaryAsync(g => g.Key, g => g.Sum(ac => ac.AdditionalCouponValue));

                additionalCoupons.ForEach(ac => ac.TotalValue = totalValuesDict.GetValueOrDefault(ac.CouponId, 0));

                return additionalCoupons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching additional coupons: {ex.Message}\n{ex.InnerException?.Message}");
                return new List<AdditionalCoupon>();
            }
        }

        public async Task<AdditionalCoupon?> AddAdditionalCouponAsync(int couponId, AdditionalCoupon additionalCoupon)
        {
            try
            {
                if (!await _db.Coupons.AnyAsync(c => c.Id == couponId))
                    return null;
                additionalCoupon.CouponId = couponId;
                await _db.AdditionalCoupons.AddAsync(additionalCoupon);
                await _db.SaveChangesAsync();
                return additionalCoupon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding additional coupon: {ex.Message}\n{ex.InnerException?.Message}");
                return null; 
            }
        }
    }
}
