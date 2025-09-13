using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Model.Context;

public class MySQLContext(DbContextOptions<MySQLContext> options) : DbContext(options)
{
    public DbSet<EmailLog> EmailLogs { get; set; }
} 
