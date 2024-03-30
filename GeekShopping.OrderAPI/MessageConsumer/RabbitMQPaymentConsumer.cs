﻿using System.Text;
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
    private const string ExchangeName = "DirectPaymentUpdateExchange";
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";
    private const string RoutingKeyPaymentOrder = "PaymentOrder";

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

        channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
        channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, RoutingKeyPaymentOrder);
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

        channel.BasicConsume(PaymentOrderUpdateQueueName, false, consumer);
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
