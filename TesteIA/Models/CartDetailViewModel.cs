using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TesteIA.Models;

public class CartDetailViewModel
{
    public long  Id { get; set; }
    public long CartHeaderId { get; set; }
    public CartHeaderViewModel? CartHeader { get; set; }
    public long ProductId { get; set; }
    public ProductViewModel? Product { get; set; }
    public int Count { get; set; }
}
