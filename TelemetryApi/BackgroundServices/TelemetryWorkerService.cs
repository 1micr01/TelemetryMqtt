using System.Text;
using System.Text.Json;
using MQTTnet.Client;
using TelemetryApi.Models;
using TelemetryApi.Services;

namespace TelemetryApi.BackgroundServices;

public class TelemetryWorkerService(ILogger<TelemetryWorkerService> logger, IMqttService mqttService, IInfluxDbService influxDbService) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await mqttService.Connect();
        await mqttService.Subscribe("dht", OnMessageReceived);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!mqttService.ClientIsConnected)
        {
            await mqttService.Reconnect();
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        Telemetry? telemetry = JsonSerializer.Deserialize<Telemetry>(Encoding.Default.GetString(eventArgs.ApplicationMessage.PayloadSegment));

        logger.LogInformation($"Temperature: {telemetry?.Temperature}");
        logger.LogInformation($"Humidity: {telemetry?.Humidity}");

        if (telemetry != null)
            await influxDbService.Write(telemetry);

    }
}