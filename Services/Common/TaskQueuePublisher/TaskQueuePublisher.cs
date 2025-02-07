using MailCrafter.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MailCrafter.Services;

public class TaskQueuePublisher : ITaskQueuePublisher, IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ILogger<TaskQueuePublisher> _logger;

    public TaskQueuePublisher(IConfiguration configuration, ILogger<TaskQueuePublisher> logger)
    {
        _logger = logger;
        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(configuration["RabbitMQ:ConnectionURI"] ?? ""),
        };
        _queueName = configuration["RabbitMQ:QueueName"] ?? string.Empty;
    }

    public async Task PublishMessageAsync(WorkerTaskMessage message)
    {
        if (_channel == null || _connection == null)
        {
            _logger.LogWarning("No active RabbitMQ connection or channel. Attempting to reconnect...");
            await ConnectToRabbitMqAsync();
        }

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await _channel!.BasicPublishAsync(
            exchange: "",
            routingKey: _queueName,
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
    private async Task ConnectToRabbitMqAsync()
    {
        int retryCount = 5;
        while (retryCount > 0)
        {
            try
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                break;
            }
            catch (Exception ex)
            {
                retryCount--;
                _logger.LogError($"Error connecting to RabbitMQ: {ex.Message}. Retries left: {retryCount}");
                if (retryCount == 0)
                    throw;
                await Task.Delay(2000);
            }
        }
    }
}
