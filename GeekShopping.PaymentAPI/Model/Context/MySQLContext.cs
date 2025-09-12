using Microsoft.EntityFrameworkCore;

namespace GeekShopping.PaymentAPI.Model.Base;

public class MySQLContext(DbContextOptions<MySQLContext> options) : DbContext(options)
{
    //public DbSet<OrderHeader> Headers { get; set; }
    //public DbSet<OrderDetail> Details { get; set; }

} 
