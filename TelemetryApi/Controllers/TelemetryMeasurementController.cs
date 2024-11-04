using Microsoft.AspNetCore.Mvc;
using TelemetryApi.Models;
using TelemetryApi.Services;

namespace TelemetryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TelemetryMeasurementController(IInfluxDbService influxDbService) : ControllerBase
{
    [HttpGet]
    public List<TelemetryMeasurement> GetList()
    {
        return influxDbService.Read();
    }
}