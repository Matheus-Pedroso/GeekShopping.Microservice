using AutoMapper;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository;

public class ProductRepository(MySQLContext context, IMapper mapper) : IProductRepository
{
    public async Task<IEnumerable<ProductVO>> FindAll()
    {
        List<Product> products = await context.Products.ToListAsync();
        return mapper.Map<List<ProductVO>>(products);
    }
    public async Task<ProductVO> FindById(long id)
    {
        Product product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
        return mapper.Map<ProductVO>(product);
    }
    public async Task<ProductVO> Create(ProductVO productVO)
    {
        Product newProduct = mapper.Map<Product>(productVO);
        context.Products.Add(newProduct);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(newProduct);
    }
    public async Task<ProductVO> Update(ProductVO productVO)
    {
        Product updateProduct = mapper.Map<Product>(productVO);
        context.Products.Update(updateProduct);
        await context.SaveChangesAsync();
        return mapper.Map<ProductVO>(updateProduct);
    }
    public async Task<bool> Delete(long id)
    {
        try
        {
            Product product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return false;
                
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
