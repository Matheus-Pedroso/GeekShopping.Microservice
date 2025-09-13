using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName ";
    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);

            channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);

            channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");
            channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

            byte[] body = GetMessageAsByteArray(message);

            channel.BasicPublish(ExchangeName, "PaymentEmail", null, body); 
            channel.BasicPublish(ExchangeName, "PaymentOrder", null, body); 
        }
    }

    // Method to convert the message to a byte array
    private byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // Ident for better readability
        };

        var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)message, options);
        var body = Encoding.UTF8.GetBytes(json);

        return body;
    }


    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                Port = 5672,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("O serviço de pagamento não está disponível no momento. Tente novamente mais tarde.", ex);
        }
    }

    private bool ConnectionExists()
    {
        if (_connection == null || !_connection.IsOpen)
            CreateConnection();

        return _connection != null && _connection.IsOpen;
    }
}
