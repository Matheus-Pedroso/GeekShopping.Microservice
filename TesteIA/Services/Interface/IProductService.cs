using TesteIA.Models;

namespace TesteIA.Services.Interface;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> FindAll();
}
