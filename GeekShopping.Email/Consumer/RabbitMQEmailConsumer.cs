using System.Text;
using System.Text.Json;
using GeekShopping.CartAPI.Repository;
using GeekShopping.Email.Messages;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.Email.MessageConsumer;

public class RabbitMQEmailConsumer : BackgroundService
{
    private readonly EmailRepository _emailRepository;

    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    private IModel _channel;
    private const string ExchangeName = "FanoutPaymentUpdateExchange";
    string queueName = "";

    public RabbitMQEmailConsumer(EmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672,
            UserName = _username,
            Password = _password
        };

        _connection = TryConnectWithPolly(factory);
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);
        queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, ExchangeName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);

            await ProccessLogs(message);

            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume(queueName, autoAck: false, consumer);

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

    private async Task ProccessLogs(UpdatePaymentResultMessage message)
    {
        try
        {
            await _emailRepository.LogEmail(message);
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao enviar email", ex);
        }
    }
}
