namespace TesteIA.Models;

public class CartViewModel // Not Mapped to a table in the database
{
    public CartHeaderViewModel CartHeader { get; set; }
    public IEnumerable<CartDetailViewModel> CartDetails { get; set; }
}
