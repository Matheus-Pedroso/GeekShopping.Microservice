using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services;

public class ProductService(HttpClient httpClient) : IProductService
{ 
    private readonly HttpClient _client = httpClient;
    public const string BasePath = "api/v1/Product";

    public async Task<IEnumerable<ProductModel>> FindAll()
    {
        var response = await _client.GetAsync(BasePath);
        return await response.ReadContentAs<List<ProductModel>>();
    }
    public async Task<ProductModel> FindById(long id)
    {
        var response = await _client.GetAsync($"{BasePath}/{id}");
        return await response.ReadContentAs<ProductModel>();
    }
    public async Task<ProductModel> Create(ProductModel model)
    {
        var response = await _client.PostAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return await response.ReadContentAs<ProductModel>();
    }
    public async Task<ProductModel> Update(ProductModel model)
    {
        var response = await _client.PutAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return await response.ReadContentAs<ProductModel>();
    }
    public async Task<bool> Delete(long id)
    {
        var response = await _client.DeleteAsync($"{BasePath}/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return true;
    }
}
