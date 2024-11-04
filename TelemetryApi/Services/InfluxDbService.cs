using InfluxDB.Client;
using InfluxDB.Client.Linq;
using Microsoft.Extensions.Options;
using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class InfluxDbService : IInfluxDbService
{
    private readonly InfluxDBClient _client;
    private readonly InfluxDbInstance _influxDbSettings;

    public InfluxDbService(IOptions<AppSettings> appSettings)
    {
        _influxDbSettings = appSettings.Value.InfluxDbInstance;
        _client = new InfluxDBClient(new InfluxDBClientOptions(_influxDbSettings.Host)
        {
            Bucket = _influxDbSettings.Bucket,
            Token = _influxDbSettings.ApiToken,
            Org = _influxDbSettings.OrganizationId
        });
    }

    public async Task Write(Telemetry telemetry)
    {
        WriteApiAsync writeApi = _client.GetWriteApiAsync();
        await writeApi.WriteMeasurementAsync(new TelemetryMeasurement() { Device = "dht", Temperature = telemetry.Temperature, Humidity = telemetry.Humidity});
    }

    public List<TelemetryMeasurement> Read()
    {
        QueryApiSync queryApi = _client.GetQueryApiSync();

        return InfluxDBQueryable<TelemetryMeasurement>.Queryable(_influxDbSettings.Bucket, _influxDbSettings.OrganizationId, queryApi).ToList();
    }
}