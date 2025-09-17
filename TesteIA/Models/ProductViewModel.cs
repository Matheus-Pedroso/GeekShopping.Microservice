using System.ComponentModel.DataAnnotations;

namespace TesteIA.Models;

public class ProductViewModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }

    [Range(1, 100)]
    public int Count { get; set; } = 1;

    // Methods to return substrings for display purposes
    public string SubstringName => string.IsNullOrEmpty(Name) ? string.Empty : Name.Length < 24 ? Name : $"{Name.Substring(0, 21)} ...";
    public string SubstringDescription => string.IsNullOrEmpty(Description) ? string.Empty : Description.Length < 355 ? Description : $"{Description.Substring(0, 352)} ...";
}
