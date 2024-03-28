using System.Text;
using System.Text.Json;
using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;

namespace GeekShopping.CartAPI.RabbitMQSender;

public class RabbitMQMessageSender : IRabbitMQMessageSender
{
    private readonly string hostName;
    private readonly string password;
    private readonly string userName;
    private IConnection connection;

    public RabbitMQMessageSender()
    {
        this.hostName = "localhost";
        this.password = "guest";
        this.userName = "guest";
    }

    public void SendMessage(BaseMessage message, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
        };
        connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
        byte[] body = GeetMessageAsByteArray(message);
        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }

    private byte[] GeetMessageAsByteArray(BaseMessage message)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize<CheckoutHeaderVO>((CheckoutHeaderVO)message, options);
        return Encoding.UTF8.GetBytes(json);
    }
}

