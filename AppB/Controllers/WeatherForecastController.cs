using System.Text.Json;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace AppB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
            })
            .ToArray();

            _logger.LogInformation("Generated forecast {forecast} with average temp {temp}", 
                string.Join(',', forecast.Select(x => x.TemperatureC)),
                forecast.Sum(x => x.TemperatureC) / forecast.Length
                );

            return forecast;
        }
    }
}