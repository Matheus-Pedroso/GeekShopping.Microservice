using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Context;

public class MySQLContext(DbContextOptions<MySQLContext> options) : DbContext(options)
{
    public DbSet<OrderHeader> Headers { get; set; }
    public DbSet<OrderDetail> Details { get; set; }

} 
