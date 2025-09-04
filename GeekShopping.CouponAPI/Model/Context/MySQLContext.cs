using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Model.Context;

public class MySQLContext(DbContextOptions<MySQLContext> options) : DbContext(options)
{
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 1,
            CouponCode = "10OFF",
            DiscountAmount = 10,
        });

        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            Id = 2,
            CouponCode = "20OFF",
            DiscountAmount = 20,
        });
    }
} 
