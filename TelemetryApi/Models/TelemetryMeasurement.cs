using InfluxDB.Client.Core;

namespace TelemetryApi.Models;

[Measurement("telemetry-measurement")]
public class TelemetryMeasurement
{
    [Column(IsTimestamp = true)]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    [Column("device", IsTag = true)]
    public string? Device { get; set; }

    [Column("temperature")]
    public double Temperature { get; set; }

    [Column("humidity")]
    public double Humidity { get; set; }
}