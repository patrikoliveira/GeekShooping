using AutoMapper;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class CartRepository : ICartRepository
{
    private readonly MySQLContext context;
    private readonly IMapper mapper;

    public CartRepository(MySQLContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        var header = await context.CartHeaders
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (header != null)
        {
            header.CouponCode = couponCode;

            context.CartHeaders.Update(header);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await context.CartHeaders
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeader != null)
        {
            context.CartDetails.RemoveRange(context.CartDetails.Where(
                        c => c.CartHeaderId == cartHeader.Id
                    )
                );

            context.CartHeaders.Remove(cartHeader);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        Cart cart = new()
        {
            CartHeader = await context.CartHeaders
                .FirstOrDefaultAsync(c => c.UserId == userId) ?? new CartHeader(),
        };

        cart.CartDetails = context.CartDetails
            .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product);

        return mapper.Map<CartVO>(cart);
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        var header = await context.CartHeaders
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (header != null)
        {
            header.CouponCode = "";

            context.CartHeaders.Update(header);
            await context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {        
        try
        {
            CartDetail cartDetail = await context.CartDetails
                .FirstOrDefaultAsync(c => c.Id == cartDetailsId);
            int total = context.CartDetails
                .Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

            context.CartDetails.Remove(cartDetail);

            if (total == 1)
            {
                var cartHeaderToRemove = await context.CartHeaders
                    .FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);
                context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<CartVO> SaveOrUpdateCart(CartVO vo)
    {
        Cart cart = mapper.Map<Cart>(vo);
        var product = await context.Products.FirstOrDefaultAsync(p =>
            p.Id == vo.CartDetails.FirstOrDefault().ProductId
        );

        if (product == null)
        {
            context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await context.SaveChangesAsync();
        }

        var cartHeader = await context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
            c => c.UserId == cart.CartHeader.UserId
            );

        if (cartHeader == null)
        {
            context.CartHeaders.Add(cart.CartHeader);
            await context.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null;
            context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await context.SaveChangesAsync();
        }
        else
        {
            var cartDetail = await context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    p.CartHeaderId == cartHeader.Id
                );

            if (cartDetail == null)
            {
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await context.SaveChangesAsync();
            }
            else
            {
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                await context.SaveChangesAsync();
            }
        }

        return mapper.Map<CartVO>(cart);
    }
}

