
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace gemini_asp_net_integration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

            // Add services to the container.
            builder.Services.AddSingleton<IKernelBuilder>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder();

                kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId: builder.Configuration["Google:ModelId"], apiKey: builder.Configuration["Google:ApiKey"]);

                return kernelBuilder;
            });

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", true);
            AppContext.SetSwitch("System.Net.Security.DisableCertificateRevocationCheck", true);


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
    }
}
