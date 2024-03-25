using AutoMapper;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository;

public class ProductRepository : IProductRepository
{
    private readonly MySQLContext context;
    private IMapper mapper;

    public ProductRepository(MySQLContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<ProductVO>> FindAll()
    {
        List<Product> products = await context.Products.ToListAsync();
        return mapper.Map<List<ProductVO>>(products);
    }

    public async Task<ProductVO> FindById(long id)
    {
        Product? product = await context.Products
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();
        return mapper.Map<ProductVO>(product);
    }

    public async Task<ProductVO> Create(ProductVO vo)
    {
        Product product = mapper.Map<Product>(vo);
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(product);
    }

    public async Task<ProductVO> Update(ProductVO vo)
    {
        Product product = mapper.Map<Product>(vo);
        context.Products.Update(product);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(product);
    }

    public async Task<bool> Delete(long id)
    {
        try
        {
            Product? product = await context.Products
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return false;
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


}

