namespace GeekShopping.CartAPI.Model;

public class Cart // Not Mapped to a table in the database
{
    public CartHeader CartHeader { get; set; }
    public IEnumerable<CartDetail> CartDetails { get; set; }
}
