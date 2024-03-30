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
    private IConnection connection;

    public RabbitMQMessageSender()
    {
        this.hostName = "localhost";
        this.password = "guest";
        this.userName = "guest";
    }

    public void SendMessage(BaseMessage message, string queueName)
    {
        if (ConnectionExists())
        {
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
            byte[] body = GeetMessageAsByteArray(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
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

