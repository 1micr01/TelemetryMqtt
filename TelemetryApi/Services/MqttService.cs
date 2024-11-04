using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class MqttService(MqttFactory mqttFactory, IMqttClient mqttClient, MqttClientOptionsBuilder builder, IOptions<AppSettings> options) : IMqttService
{
    private readonly IOptions<AppSettings> _options = options;

    public bool ClientIsConnected => mqttClient.IsConnected;

    public async Task<MqttClientConnectResult> Connect(MqttClientOptionsBuilder? optionsBuilder = null, CancellationToken? cancellationToken = null)
    {
        optionsBuilder ??= builder;
        return await mqttClient.ConnectAsync(optionsBuilder.Build(), cancellationToken ?? CancellationToken.None);
    }

    public async Task Reconnect(CancellationToken? cancellationToken = null)
    {
        await mqttClient.ReconnectAsync(cancellationToken ?? CancellationToken.None);
    }

    public async Task<MqttClientPublishResult> Publish(string topic, dynamic payload, bool asJson = false, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce, CancellationToken? cancellationToken = null)
    {
        if (!ClientIsConnected)
        {
            await Reconnect();
        }

        return await mqttClient.PublishAsync(new MqttApplicationMessage()
        {
            Topic = topic,
            PayloadSegment = Encoding.Default.GetBytes(asJson ? JsonSerializer.Serialize(payload) : payload.ToString()),
            QualityOfServiceLevel = qos
        }, cancellationToken ?? CancellationToken.None);
    }

    public async Task Subscribe(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> onMessageReceivedEvent, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, CancellationToken? cancellationToken = null)
    {
        MqttClientSubscribeOptions mqttClientSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(topic, qos)
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += onMessageReceivedEvent;

        if (!ClientIsConnected)
        {
            await Reconnect();
        }

        await mqttClient.SubscribeAsync(mqttClientSubscribeOptions, cancellationToken ?? CancellationToken.None);
    }

    public MqttClientOptionsBuilder GetOptionsBuilder()
    {
        return builder;
    }

    public IMqttClient GetClient()
    {
        return mqttClient;
    }
}