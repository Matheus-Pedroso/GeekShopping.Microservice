namespace GeekShopping.OrderAPI.Messages;

public class CheckoutHeaderVO
{
    public string UserId { get; set; } = string.Empty;
    public string? CouponCode { get; set; } = string.Empty;
    public decimal PurchaseAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.Now;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
    public string ExpiryMonthYear { get; set; } = string.Empty;
    public int CartTotalItens { get; set; }
    public List<CartDetailVO> CartDetails { get; set; } = new List<CartDetailVO>();
}
