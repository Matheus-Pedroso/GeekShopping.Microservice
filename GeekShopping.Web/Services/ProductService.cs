using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interface;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services;

public class ProductService(HttpClient httpClient) : IProductService
{ 
    private readonly HttpClient _client = httpClient;
    public const string BasePath = "api/v1/Product";

    public async Task<IEnumerable<ProductViewModel>> FindAll()
    {
        var response = await _client.GetAsync(BasePath);
        return await response.ReadContentAs<List<ProductViewModel>>();
    }
    public async Task<ProductViewModel> FindById(long id)
    {
        var response = await _client.GetAsync($"{BasePath}/{id}");
        return await response.ReadContentAs<ProductViewModel>();
    }
    public async Task<ProductViewModel> Create(ProductViewModel model)
    {
        var response = await _client.PostAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return await response.ReadContentAs<ProductViewModel>();
    }
    public async Task<ProductViewModel> Update(ProductViewModel model)
    {
        var response = await _client.PutAsJson(BasePath, model);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return await response.ReadContentAs<ProductViewModel>();
    }
    public async Task<bool> Delete(long id)
    {
        var response = await _client.DeleteAsync($"{BasePath}/{id}");
        if (!response.IsSuccessStatusCode)
            throw new Exception("Something went wrong when calling API");

        return true;
    }
}
