using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WeatherForecast.Api.Controllers;

[ApiController]
[Route("api/weatherforecast"), Authorize]
public class WeatherForecastController: ControllerBase {

	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	private readonly ILogger<WeatherForecastController> _logger;

	public WeatherForecastController(ILogger<WeatherForecastController> logger) {
		_logger = logger;
	}

	[HttpGet, EnforcePolicy("demo.weatherapi.allow")]
	public IEnumerable<Models.WeatherForecast> Get() {
		return Enumerable.Range(1, 5).Select(index => new Models.WeatherForecast {
			Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		})
		.ToArray();
	}
}
