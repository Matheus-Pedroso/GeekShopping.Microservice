using System.Text;
using System.Text.Json;
using GeekShopping.CartAPI.Repository;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private readonly IRabbitMQMessageSender _messageSender;

    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    private IModel _channel;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName ";


    public RabbitMQPaymentConsumer(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672,
            UserName = _username,
            Password = _password
        };

        _connection = TryConnectWithPolly(factory);
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);
        _channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
        _channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultVO vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);

            await UpdatePaymentStatus(vo);

            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(PaymentOrderUpdateQueueName, autoAck: false, consumer);

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

    private async Task UpdatePaymentStatus(UpdatePaymentResultVO vo)
    {
        try
        {
            await _orderRepository.UpdateOrderPaymentStatus(vo.OrderId, vo.Status);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao atualizar o status do pagamento", ex);
        }
    }
}
