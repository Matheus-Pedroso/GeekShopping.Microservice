using System.Text;
using System.Text.Json;
using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    public void SendMessage(BaseMessage message, string queueName)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);
            byte[] body = GetMessageAsByteArray(message);
            channel.BasicPublish("", queueName, null, body); 
        }
    }

    // Method to convert the message to a byte array
    private byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true // Ident for better readability
        };

        var json = JsonSerializer.Serialize<CheckoutHeaderVO>((CheckoutHeaderVO)message, options);
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
            throw new ApplicationException("O serviço de pedidos não está disponível no momento. Tente novamente mais tarde.", ex);
        }
    }

    private bool ConnectionExists()
    {
        if (_connection == null || !_connection.IsOpen)
            CreateConnection();

        return _connection != null && _connection.IsOpen;
    }
}
