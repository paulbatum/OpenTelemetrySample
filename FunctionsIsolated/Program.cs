using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddLogging((logging) =>
        {
            logging.AddOpenTelemetry(ot =>
            {
                ot.AddConsoleExporter();
            });
        });
    })
    .Build();

host.Run();
