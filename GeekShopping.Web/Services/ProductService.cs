using System.Net.Http.Headers;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services;

public class ProductService : IProductService
{
    private readonly HttpClient client;
    public const string BasePath = "api/v1/product";

    public ProductService(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IEnumerable<ProductModel>> FindAllProducts(string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync(BasePath);
        return await response.ReadContentAs<List<ProductModel>>();
    }

    public async Task<ProductModel> FindProductById(long id, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync($"{BasePath}/{id}");
        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<ProductModel> CreateProduct(ProductModel model, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.PostAsJson(BasePath, model);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong when calling API");
        }

        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<ProductModel> UpdateProduct(ProductModel model, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.PutAsJson(BasePath, model);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong when calling API");
        }

        return await response.ReadContentAs<ProductModel>();
    }

    public async Task<bool> DeleteProductById(long id, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.DeleteAsync($"{BasePath}/{id}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong when calling API");
        }

        return await response.ReadContentAs<bool>();
    }
}

