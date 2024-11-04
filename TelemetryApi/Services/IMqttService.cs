using MQTTnet.Client;
using MQTTnet.Protocol;

namespace TelemetryApi.Services;

public interface IMqttService
{
    Task<MqttClientConnectResult> Connect(MqttClientOptionsBuilder? optionsBuilder = null, CancellationToken? cancellationToken = null);
    Task Reconnect(CancellationToken? cancellationToken = null);
    Task<MqttClientPublishResult> Publish(string topic, dynamic payload, bool asJson = false, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtMostOnce, CancellationToken? cancellationToken = null);
    Task Subscribe(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> onMessageReceivedEvent, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce, CancellationToken? cancellationToken = null);
    MqttClientOptionsBuilder GetOptionsBuilder();
    IMqttClient GetClient();
    bool ClientIsConnected { get; }
}