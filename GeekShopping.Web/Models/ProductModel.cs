using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace GeekShopping.Web.Models;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }

    // Methods to return substrings for display purposes
    public string SubstringName => Name.Length < 24 ? Name : $"{Name.Substring(0, 21)} ...";
    public string SubstringDescription => Description.Length < 355 ? Description : $"{Description.Substring(0, 352)} ...";
}
