using System.Text;
using System.Text.Json;
using GeekShopping.PaymentAPI.Messages;
using GeekShopping.PaymentAPI.RabbitMQSender;
using GeekShopping.PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.PaymentAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private IConnection connection;
    private IModel channel;
    private IRabbitMQMessageSender rabbitMQMessageSender;
    private IProcessPayment processPayment;
    private const string orderPaymentProcessQueue = "orderpaymentprocessqueue";
    private const string orderPaymentResultQueue = "orderpaymentresultqueue";
    
    public RabbitMQPaymentConsumer(IProcessPayment processPayment, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        this.processPayment = processPayment ?? throw new ArgumentNullException(nameof(processPayment));
        this.rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(queue: orderPaymentProcessQueue, false, false, false, arguments: null);
            }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            PaymentMessage vo = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPayment(vo).GetAwaiter().GetResult();
            this.channel.BasicAck(evt.DeliveryTag, false);
        };

        channel.BasicConsume(orderPaymentProcessQueue, false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessPayment(PaymentMessage vo)
    {
        try
        {
            var result = processPayment.PaymentProcessor();
            UpdatePaymentResultMessage paymentResult = new()
            {
                Status = result,
                OrderId = vo.OrderId,
                Email = vo.Email,
            };
            rabbitMQMessageSender.SendMessage(paymentResult);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

