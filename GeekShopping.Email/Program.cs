
using System.Security.Claims;
using GeekShopping.CartAPI.Repository;
using GeekShopping.Email.MessageConsumer;
using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GeekShopping.Email
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder);
            ConfigureAuthentication(builder);

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
            app.UseAuthentication();
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

        private static void ConfigureAuthentication(WebApplicationBuilder services)
        {
            // Add Authentication
            services.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:4435"; // Identity Server
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                    };
                });

            // Add Authorization
            services.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "geek_shopping");
                });
            });
        }
    }
}
