using Microsoft.AspNetCore.Mvc;

namespace AppA.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            using var response = await _appBClient.GetAsync("/WeatherForecast");
            response.EnsureSuccessStatusCode();
            var forecast = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
            return forecast;            
        }
    }
}