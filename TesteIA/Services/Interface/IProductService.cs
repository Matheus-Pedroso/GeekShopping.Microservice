using TesteIA.Models;

namespace TesteIA.Services.Interface;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> FindAll();
    Task<ProductViewModel> FindById(long id);
    Task<ProductViewModel> Create(ProductViewModel model);
    Task<ProductViewModel> Update(ProductViewModel model);
    Task<bool> Delete(long id);
}
