using System.ComponentModel;
using TesteIA.Services.Interface;
using Microsoft.SemanticKernel;
using TesteIA.Models;

namespace TesteIA.Plugins;

public class ProductPlugin(IProductService productService)
{
    private readonly List<Product> _products = 
        [
            new(Id: 1, Name: "Laptop", IsActive: true, Quantity: 50),
            new(Id: 2, Name: "Smartphone", IsActive: true, Quantity: 200),
            new(Id: 3, Name: "Tablet", IsActive: false, Quantity: 0),
            new(Id: 4, Name: "Monitor", IsActive: true, Quantity: 1),
            new(Id: 5, Name: "Keyboard", IsActive: true, Quantity: 150)
        ];
    private readonly IProductService _productService = productService;

    [KernelFunction("get_products")]
    [Description("Returns list of products")]
    public async Task<List<ProductViewModel>> GetAllProducts()
    {
        var products = await _productService.FindAll();
        return products.ToList();
    }
}

public record Product
(
     int Id,
     string Name,
     bool IsActive,
     int Quantity
);
