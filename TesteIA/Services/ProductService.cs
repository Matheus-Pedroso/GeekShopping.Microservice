using TesteIA.Services.Interface;
using TesteIA.Models;
using TesteIA.Utils;

namespace TesteIA.Services;

public class ProductService(HttpClient httpClient) : IProductService
{ 
    private readonly HttpClient _client = httpClient;
    public const string BasePath = "api/v1/Product";

    public async Task<IEnumerable<ProductViewModel>> FindAll()
    {
        var response = await _client.GetAsync(BasePath);
        return await response.ReadContentAs<List<ProductViewModel>>();
    }
}
