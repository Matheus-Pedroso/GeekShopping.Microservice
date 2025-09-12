using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    private IModel _channel;
    private IRabbitMQMessageSender _rabbitMQMessageSender;
    private readonly IProcessPayment _processPayment;

    public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        _processPayment = processPayment;
        rabbitMQMessageSender = _rabbitMQMessageSender;
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672,
            UserName = _username,
            Password = _password
        };
        _connection = TryConnectWithPolly(factory);
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "orderpaymentprocessqueue", false, false, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            PaymentMessage vo = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProccessPayment(vo).GetAwaiter().GetResult();
            _channel.BasicAck(evt.DeliveryTag, false);
        };
        _channel.BasicConsume("orderpaymentprocessqueue", false, consumer);

        return Task.CompletedTask;
    }

    private IConnection TryConnectWithPolly(ConnectionFactory factory)
    {
        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                Console.WriteLine($"[RabbitMQ Consumer] Tentando se reconectar após {time.TotalSeconds} segundos: {ex.Message}");
            });

        return policy.Execute(() => factory.CreateConnection());
    }

    private async Task ProccessPayment(PaymentMessage vo)
    {
        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage paymentResult = new UpdatePaymentResultMessage()
        {
            Status = result,
            OrderId = vo.OrderId,
            Email = vo.Email
        };
        try
        {
            _rabbitMQMessageSender.SendMessage(paymentResult, "orderpaymentresultqueue");
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao processar o pagamento do pedido", ex);
        }
    }
}
