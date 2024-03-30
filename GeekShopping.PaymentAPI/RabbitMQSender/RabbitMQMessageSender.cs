using System.Text;
using System.Text.Json;
using GeekShopping.MessageBus;
using GeekShopping.PaymentAPI.Messages;
using RabbitMQ.Client;

namespace GeekShopping.PaymentAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string hostName;
    private readonly string password;
    private readonly string userName;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
    private const string RoutingKeyPaymentEmail = "PaymentEmail";
    private const string RoutingKeyPaymentOrder = "PaymentOrder";
    private IConnection connection;

    public RabbitMQMessageSender()
    {
        this.hostName = "localhost";
        this.password = "guest";
        this.userName = "guest";
    }

    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);

            channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);

            channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, RoutingKeyPaymentEmail);
            channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, RoutingKeyPaymentOrder);

            byte[] body = GeetMessageAsByteArray(message);
            channel.BasicPublish(exchange: ExchangeName, RoutingKeyPaymentEmail, basicProperties: null, body: body);
            channel.BasicPublish(exchange: ExchangeName, RoutingKeyPaymentOrder, basicProperties: null, body: body);
        }
    }

    private byte[] GeetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)message, options);
        return Encoding.UTF8.GetBytes(json);
    }

    private bool ConnectionExists()
    {
        if (connection != null)
        {
            return true;
        }
        CreateConnection();
        return connection != null;
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
            };
            connection = factory.CreateConnection();
        }
        catch (Exception)
        {
            throw;
        }
    }
}

