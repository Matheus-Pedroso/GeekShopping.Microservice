using Microsoft.AspNetCore.Authentication;
using Microsoft.SemanticKernel;
using TesteIA.Plugins;
using TesteIA.Services;
using TesteIA.Services.Interface;
using TesteIA.Utils;

namespace TesteIA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddKernel()
                .AddOllamaChatCompletion(
                    modelId: "llama3.1:latest",
                    endpoint: new Uri("http://localhost:11434")
                );

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<AccessTokenHandler>();

            // ProductService
            builder.Services.AddHttpClient<IProductService, ProductService>(c =>
            {
                c.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]);
            }).AddHttpMessageHandler<AccessTokenHandler>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:4435"; // sem barra no final
                    options.RequireHttpsMetadata = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClientId = "geek_shopping";
                    options.ClientSecret = "my_secret";
                    options.ResponseType = "code";
                    options.ClaimActions.MapJsonKey("role", "role", "role");
                    options.ClaimActions.MapJsonKey("sub", "sub", "sub");
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                    options.Scope.Add("geek_shopping");
                    options.SaveTokens = true;
                });


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
    }
}
