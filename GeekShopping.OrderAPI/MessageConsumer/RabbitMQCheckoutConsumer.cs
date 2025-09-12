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

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private readonly IRabbitMQMessageSender _messageSender;

    private readonly string _hostname = "localhost";
    private readonly string _username = "guest";
    private readonly string _password = "guest";
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IRabbitMQMessageSender messageSender)
    {
        _orderRepository = orderRepository;
        _messageSender = messageSender;
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = 5672,
            UserName = _username,
            Password = _password
        };

        _connection = TryConnectWithPolly(factory);
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "checkoutqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);

            await ProccessOrder(vo);

            _channel.BasicAck(evt.DeliveryTag, false);
        };

        _channel.BasicConsume("checkoutqueue", autoAck: false, consumer);

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

    private async Task ProccessOrder(CheckoutHeaderVO vo)
    {
        OrderHeader order = new()
        {
            UserId = vo.UserId,
            FirstName = vo.FirstName,
            LastName = vo.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = vo.CardNumber,
            CouponCode = vo.CouponCode,
            CVV = vo.CVV,
            DiscountAmount = vo.DiscountAmount,
            Email = vo.Email,
            ExpiryMonthYear = vo.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = vo.PurchaseAmount,
            PaymentStatus = false,
            Phone = vo.Phone,
            DateTime = vo.Time
        };

        foreach (var item in vo.CartDetails)
        {
            OrderDetail detail = new()
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Price = item.Product.Price,
                Count = item.Count
            };
            order.CartTotalItens += item.Count;
            order.OrderDetails.Add(detail);
        }

        await _orderRepository.AddOrder(order);

        ProcessPayment(order);
    }

    private void ProcessPayment(OrderHeader order)
    {
        PaymentVO paymentVO = new()
        {
            Name = order.FirstName + " " + order.LastName,
            Email = order.Email,
            CardNumber = order.CardNumber,
            CVV = order.CVV,
            ExpiryMonthYear = order.ExpiryMonthYear,
            OrderId = order.Id,
            PurchaseAmount = order.PurchaseAmount,
        };

        try
        {
            _messageSender.SendMessage(paymentVO, "orderpaymentprocessqueue");
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao processar o pagamento do pedido", ex);
        }
    }
}
