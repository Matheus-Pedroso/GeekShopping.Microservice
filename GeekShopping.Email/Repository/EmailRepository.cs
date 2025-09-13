using GeekShopping.Email.Messages;
using GeekShopping.Email.Model;
using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository;

public class EmailRepository(DbContextOptions<MySQLContext> context) : IEmailRepository
{
    public async Task LogEmail(UpdatePaymentResultMessage message)
    {
        EmailLog email = new EmailLog()
        {
            Email = message.Email,
            SentDate = DateTime.Now,
            Log = $"Order ID: {message.OrderId} has been created successfully"
        };
        await using var _db = new MySQLContext(context);
        _db.EmailLogs.Add(email);
        await _db.SaveChangesAsync();
    }
}
