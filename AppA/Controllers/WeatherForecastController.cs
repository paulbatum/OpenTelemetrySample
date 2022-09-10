using Microsoft.AspNetCore.Mvc;

namespace AppA.Controllers
{
    [ApiController]
    [Route("WeatherForecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _appBClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _appBClient = httpClientFactory.CreateClient("AppB");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            using var response1 = await _appBClient.GetAsync("/WeatherForecast");
            response1.EnsureSuccessStatusCode();
            var forecast1 = await response1.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();

            using var response2 = await _appBClient.GetAsync("/WeatherForecast");
            response2.EnsureSuccessStatusCode();
            var forecast2 = await response2.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();

            var averagetemp1 = forecast1.Sum(x => x.TemperatureC) / forecast1.Count();
            var averagetemp2 = forecast2.Sum(x => x.TemperatureC) / forecast2.Count();

            if (averagetemp1 > averagetemp2)
            {
                _logger.LogInformation("Using forecast1 because its warmer, with average temp of {temp}", averagetemp1);
                return forecast1;
            }
            else
            {
                _logger.LogInformation("Using forecast2 because its warmer, with average temp of {temp}", averagetemp2);
                return forecast2;
            }
        }
    }
}