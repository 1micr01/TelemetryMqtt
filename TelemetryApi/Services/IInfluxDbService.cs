using TelemetryApi.Models;

namespace TelemetryApi.Services;

public interface IInfluxDbService
{
    Task Write(Telemetry telemetry);
    List<TelemetryMeasurement> Read();
}