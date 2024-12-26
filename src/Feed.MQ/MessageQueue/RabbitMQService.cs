using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Feed.MQ.MessageQueue
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly RabbitMQSettings _rabbitMQSettings;
        public RabbitMQService(ILogger<RabbitMQService> logger, IOptions<RabbitMQSettings> rabbitMQSettings
            
            )
        {
            _logger = logger;
            _rabbitMQSettings = rabbitMQSettings.Value;

        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.Host,
                    Port = _rabbitMQSettings.Port,
                    UserName = _rabbitMQSettings.UserName,
                    Password = _rabbitMQSettings.Password,
                };
                using var _connection = await factory.CreateConnectionAsync();
                using var _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
                var serializedMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(serializedMessage);

                var props = new BasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = (DeliveryModes)2;
                props.Expiration = "36000000";

                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: props,
                    body: body);

                await Task.CompletedTask;
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to queue {QueueName}", queueName);
                throw;
            }
        }

        public async Task ConsumeAsync<T>(string queueName, Func<T, Task> handler)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.Host ?? "localhost",
                    Port = _rabbitMQSettings.Port,
                    UserName = _rabbitMQSettings.UserName ?? "guest",
                    Password = _rabbitMQSettings.Password ?? "guest",
                };
                factory.Port = 5672;
                using var _connection = await factory.CreateConnectionAsync();

                using var _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var deserializedMessage = JsonSerializer.Deserialize<T>(message);

                        if (deserializedMessage != null)
                        {
                            await handler(deserializedMessage);
                            await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up consumer for queue {QueueName}", queueName);
                throw;
            }
        }
    }
}
