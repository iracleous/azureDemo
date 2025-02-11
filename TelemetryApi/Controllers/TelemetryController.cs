using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace TelemetryApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryClient _telemetryClient;

    // Inject TelemetryClient to send telemetry data
    public TelemetryController(TelemetryClient telemetryClient)
    {
        _telemetryClient = telemetryClient;
    }

    // Endpoint to track custom event
    [HttpGet("trackEvent")]
    public IActionResult TrackEvent(string eventName)
    {
        _telemetryClient.TrackEvent(eventName);
        return Ok($"Event '{eventName}' tracked successfully.");
    }

    // Endpoint to track a custom exception
    [HttpGet("trackException")]
    public IActionResult TrackException()
    {
        try
        {
            // Simulating an exception
            throw new Exception("Sample exception");
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(ex);
            return Ok("Exception tracked.");
        }
    }

    // Endpoint to track custom metrics
    [HttpGet("trackMetric")]
    public IActionResult TrackMetric(string metricName, double value)
    {
        _telemetryClient.GetMetric(metricName).TrackValue(value);
        return Ok($"Metric '{metricName}' with value {value} tracked.");
    }
}