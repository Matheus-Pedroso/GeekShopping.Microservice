using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;
using TesteIA.Models;
using TesteIA.Services.Interface;

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
    [KernelFunction("get_product")]
    [Description("Return a product")]
    public async Task<ProductViewModel> GetProduct(long id)
    {
        var product = await _productService.FindById(id);
        return product;
    }

    [KernelFunction("create_product")]
    [Description("Create a product")]
    public async Task<ProductViewModel> CreateProduct(string Name, string Description, decimal Price, string ImageUrl, string CategoryName)
    {
        var model = new ProductViewModel
        {
            Name = Name,
            Description = Description,
            Price = Price,
            ImageUrl = ImageUrl,
            CategoryName = CategoryName
        };
        var products = await _productService.Create(model);
        return products;
    }
    [KernelFunction("update_product")]
    [Description("Update a product")]
    public async Task<ProductViewModel> UpdateProduct(long id, string Name, string Description, decimal Price, string ImageUrl, string CategoryName)
    {
        var model = new ProductViewModel
        {
            Id = id,
            Name = Name,
            Description = Description,
            Price = Price,
            ImageUrl = ImageUrl,
            CategoryName = CategoryName
        };
        var products = await _productService.Update(model);
        return products;
    }
    [KernelFunction("delete_product")]
    [Description("delete a product")]
    public async Task<bool> DeleteProduct(long id)
    {
        var products = await _productService.Delete(id);
        return products;
    }
    
}

public record Product
(
     int Id,
     string Name,
     bool IsActive,
     int Quantity
);
