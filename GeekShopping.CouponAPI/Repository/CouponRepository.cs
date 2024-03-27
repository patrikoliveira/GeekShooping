using AutoMapper;
using GeekShopping.CouponAPI.Data.ValueObjects;
using GeekShopping.CouponAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly MySQLContext context;
    private IMapper mapper;

    public CouponRepository(MySQLContext context, IMapper mapper)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<CouponVO> GetCouponByCouponCode(string couponCode)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);
        return mapper.Map<CouponVO>(coupon);
    }
}

