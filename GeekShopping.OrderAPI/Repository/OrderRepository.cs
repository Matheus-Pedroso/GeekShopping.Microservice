using GeekShopping.CartAPI.Repository.Interfaces;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class OrderRepository(DbContextOptions<MySQLContext> context) : IOrderRepository
{
    public async Task<bool> AddOrder(OrderHeader header)
    {
        if (header == null) return false;

        await using var db = new MySQLContext(context);
        db.Headers.Add(header);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateOrderPaymentStatus(long orderHeaderId, bool status)
    {
        if (orderHeaderId == 0)
            return false;

        await using var db = new MySQLContext(context);
        var header = await db.Headers.FirstOrDefaultAsync(h => h.Id == orderHeaderId);
        if (header == null) return false;
        header.PaymentStatus = status;
        await db.SaveChangesAsync();
        return true;
    }
}
