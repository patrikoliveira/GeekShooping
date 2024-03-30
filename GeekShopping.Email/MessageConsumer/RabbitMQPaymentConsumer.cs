using System.Text;
using System.Text.Json;
using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.Email.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly EmailRepository repository;
    private IConnection connection;
    private IModel channel;
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
    private const string RoutingKeyPaymentEmail = "PaymentEmail";

    public RabbitMQPaymentConsumer(EmailRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
        channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, RoutingKeyPaymentEmail);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
            ProcessLogs(message).GetAwaiter().GetResult();
            this.channel.BasicAck(evt.DeliveryTag, false);
        };

        channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessLogs(UpdatePaymentResultMessage message)
    {
        try
        {
            await repository.LogEmail(message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

