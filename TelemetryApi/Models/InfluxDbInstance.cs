namespace TelemetryApi.Models;

public class InfluxDbInstance
{
    public string Host { get; set; }
    public string ApiToken { get; set; }
    public string OrganizationId { get; set; }
    public string Bucket { get; set; }
}