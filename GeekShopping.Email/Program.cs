
using GeekShopping.CartAPI.Repository;
using GeekShopping.Email.MessageConsumer;
using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder services)
        {
            var connection = services.Configuration["MySQLConnection:MySQLConnectionString"];
            services.Services.AddDbContext<MySQLContext>(options =>
            {
                options.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 4)));
            });


            var builder = new DbContextOptionsBuilder<MySQLContext>();
            builder.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 4)));

            services.Services.AddSingleton(new EmailRepository(builder.Options));
            services.Services.AddScoped<IEmailRepository, EmailRepository>();

            services.Services.AddHostedService<RabbitMQEmailConsumer>();
        }
    }
}
