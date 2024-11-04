namespace TelemetryApi.Models;

public class AppSettings
{
    public MqttBroker MqttBroker { get; set; }

    public InfluxDbInstance InfluxDbInstance { get; set; }
}