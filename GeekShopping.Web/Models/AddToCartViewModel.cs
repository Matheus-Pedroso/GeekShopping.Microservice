using System.ComponentModel.DataAnnotations;

namespace GeekShopping.Web.Models;

public class AddToCartViewModel
{
    public long Id { get; set; }
    public int Count { get; set; }

}
