namespace GeekShopping.CartAPI.Data.ValueObjects;

public class CartVO // Not Mapped to a table in the database
{
    public CartHeaderVO CartHeader { get; set; }
    public IEnumerable<CartDetailVO>? CartDetails { get; set; } = null;
}
