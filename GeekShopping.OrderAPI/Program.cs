
using System.Security.Claims;
using GeekShopping.CartAPI.Repository;
using GeekShopping.CartAPI.Repository.Interfaces;
using GeekShopping.OrderAPI.MessageConsumer;
using GeekShopping.OrderAPI.Model.Context;
using GeekShopping.OrderAPI.RabbitMQSender;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace GeekShopping.OrderAPI
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeekShopping.CartAPI", Version = "v1" });
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            }); 

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

            services.Services.AddSingleton(new OrderRepository(builder.Options));

            services.Services.AddHostedService<RabbitMQCheckoutConsumer>();
            services.Services.AddHostedService<RabbitMQPaymentConsumer>();
            services.Services.AddSingleton<IRabbitMQMessageSender, RabbitMQMessageSender>();
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
