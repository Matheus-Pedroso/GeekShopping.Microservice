using GeekShopping.OrderAPI.Model;

namespace GeekShopping.CartAPI.Repository.Interfaces;

public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader header);
    Task<bool> UpdateOrderPaymentStatus(long orderHeaderId, bool paid);
}
