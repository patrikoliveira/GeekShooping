using System.Text;
using System.Text.Json;
using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeekShopping.OrderAPI.MessageConsumer;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private readonly OrderRepository repository;
    private IConnection connection;
    private IModel channel;
    //private const string orderPaymentResultQueue = "orderpaymentresultqueue";
    private const string ExchangeName = "FanoutPaymentUpdateExchange";
    private string queueName = "";

    public RabbitMQPaymentConsumer(OrderRepository repository)
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

        //channel.QueueDeclare(queue: orderPaymentResultQueue, false, false, false, arguments: null);
        channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
        queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, ExchangeName, "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (channel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            UpdatePaymentResultVO vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);
            UpdatePaymentStatus(vo).GetAwaiter().GetResult();
            this.channel.BasicAck(evt.DeliveryTag, false);
        };

        channel.BasicConsume(queueName, false, consumer);
        return Task.CompletedTask;
    }

    private async Task UpdatePaymentStatus(UpdatePaymentResultVO vo)
    {
        try
        {
            await repository.UpdateOrderPaymentStatus(vo.OrderId, vo.Status);
        }
        catch (Exception)
        {
            throw;
        }
    }
}

