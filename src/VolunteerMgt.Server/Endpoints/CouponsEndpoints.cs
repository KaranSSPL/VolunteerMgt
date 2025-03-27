using VolunteerMgt.Server.Abstraction.CouponService;
using VolunteerMgt.Server.Models.Coupons;

namespace VolunteerMgt.Server.Endpoints;

public static class CouponsEndpoints
{
    public static void MapCouponsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/coupons")
            .WithOpenApi();

        group.MapGet("/", async (ICouponService service) =>
            Results.Ok(await service.GetAllCouponsAsync()));

        group.MapGet("/{id}", async (int id, ICouponService service) =>
        {
            var coupon = await service.GetCouponByIdAsync(id);
            return coupon is not null ? Results.Ok(coupon) : Results.NotFound();
        });

        group.MapPost("/add", async (Coupons coupon, ICouponService service) =>
        {
            var createdCoupon = await service.AddCouponAsync(coupon);
            return Results.Created($"/api/coupons/{createdCoupon.Id}", createdCoupon);
        });

        group.MapPost("/{couponId}/additional", async (int couponId, AdditionalCoupon additionalCoupon, ICouponService service) =>
        {
            var result = await service.AddAdditionalCouponAsync(couponId, additionalCoupon);
            return result is not null ? Results.Created($"/api/coupons/{couponId}/additional/{result.Id}", result) : Results.NotFound("Coupon not found");
        });

        group.MapGet("/additional", async (ICouponService service) =>
        Results.Ok(await service.GetAllAdditionalCouponsAsync()));

    }
}
