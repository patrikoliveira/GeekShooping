using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using GeekShopping.CartAPI.Data.ValueObjects;

namespace GeekShopping.CartAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly HttpClient client;

    public CouponRepository(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<CouponVO> GetCoupon(string couponCode, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/v1/coupon/{couponCode}");
        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new CouponVO();
        }

        return JsonSerializer.Deserialize<CouponVO>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}

