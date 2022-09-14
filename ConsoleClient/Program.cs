using System.Net.Http.Json;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace ConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration(app => app.AddJsonFile("appsettings.json"))
                .ConfigureServices(services =>
                {
                    services.AddHttpClient("AppAClient", client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7178");
                        client.DefaultRequestHeaders.Add("UserID", "pbatum");
                    });
                });                

            using IHost host = builder.Build();
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("AppAClient");            

            var table = new Table();
            table.AddColumn("Day");
            table.AddColumn("TemperatureC");                        
            
            while (true)
            {                
                using var response = await client.GetAsync("/WeatherForecast");
                response.EnsureSuccessStatusCode();
                var forecast = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();

                table.Rows.Clear();
                foreach(var f in forecast)
                {
                    table.AddRow(f.Date.DayOfWeek.ToString(), f.TemperatureC.ToString());
                }
                AnsiConsole.Write(table);
                Console.ReadLine();
            }
        }
    }


}