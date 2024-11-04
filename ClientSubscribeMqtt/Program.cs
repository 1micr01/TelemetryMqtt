// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.Json;
using ClientSubscribeMqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

MqttFactory mqttFactory = new MqttFactory();

using IMqttClient mqttClient = mqttFactory.CreateMqttClient();
string brokerHost = "6d50cc68ea9d4e079719910a30d98aee.s1.eu.hivemq.cloud";
MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer(brokerHost)
    .WithCredentials("dotnet", "haSGlWNJemJOVTpR")
    .WithTlsOptions(x => x.UseTls())
    .WithProtocolVersion(MqttProtocolVersion.V311)
    .WithWillTopic("health")
    .WithWillPayload("dead")
    .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
    .WithWillRetain()
    .Build();

mqttClient.ApplicationMessageReceivedAsync += eventArgs =>
{
    Telemetry? telemetry = JsonSerializer.Deserialize<Telemetry>(Encoding.Default.GetString(eventArgs.ApplicationMessage.PayloadSegment));
    Console.WriteLine($"Telemetry");
    Console.WriteLine($"Temperature: {telemetry?.Temperature}");
    Console.WriteLine($"Humidity: {telemetry?.Humidity}");
    Console.WriteLine();

    return Task.CompletedTask;
};

await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

await mqttClient.PublishAsync(new MqttApplicationMessage()
{
    Topic = "health",
    PayloadSegment = Encoding.Default.GetBytes("alive"),
    QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce,
    Retain = true
}, CancellationToken.None);

MqttClientSubscribeOptions mqttClientSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("dht").Build();

await mqttClient.SubscribeAsync(mqttClientSubscribeOptions, CancellationToken.None);
Console.WriteLine("MQTT client subscribed to topic.");

string? inputAction;

do
{
    inputAction = Console.ReadLine();
    string? inputValue = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputAction) && !string.IsNullOrWhiteSpace(inputValue))
    {
        await mqttClient.PublishAsync(new MqttApplicationMessage()
        {
            Topic = $"action/{inputAction}",
            PayloadSegment = Encoding.Default.GetBytes(inputValue),
            QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
        }, CancellationToken.None);
    }
} while (!string.IsNullOrWhiteSpace(inputAction));


throw new NotImplementedException();