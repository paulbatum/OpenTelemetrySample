using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common
{
    public static class OpenTelemetryConfiguration
    {
        private const string OpenTelemetryEtwSession = "EtwSession=OpenTelemetry";
        public static readonly ActivitySource ActivitySource = new ActivitySource("OpenTelemetrySample");

        public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder, string applicationName)
        {
            var fields = new Dictionary<string, object> { { "pid", Environment.ProcessId } };

            //builder.Logging.AddOpenTelemetry(b =>
            //{
            //    b.AddConsoleExporter()
            //    .AddGenevaLogExporter(g =>
            //    {
            //        g.ConnectionString = OpenTelemetryEtwSession;
            //        g.PrepopulatedFields = fields;
            //    });
            //});

            builder.Services.AddOpenTelemetryTracing(t =>
                t.AddSource(ActivitySource.Name)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(applicationName))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                //.AddProcessor(new TraceBaggageEnricher())
                .AddConsoleExporter()
                .AddGenevaTraceExporter(g =>
                {
                    g.ConnectionString = OpenTelemetryEtwSession;
                    g.PrepopulatedFields = fields;
                })
                .AddOtlpExporter(o =>
                {                    
                    o.Endpoint = new Uri("http://localhost:4317");
                    o.ExportProcessorType = ExportProcessorType.Simple;
                })
            );
        }

        private class TraceBaggageEnricher : BaseProcessor<Activity>
        {
            public override void OnEnd(Activity data)
            {
                var baggageDictionary = Baggage.GetBaggage();
                foreach (var baggage in baggageDictionary)
                {
                    Debug.WriteLine($"{Process.GetCurrentProcess().ProcessName} ENRICHING via Baggage.GetBaggage {baggage.Key}:{baggage.Value}");
                    data.SetTag(baggage.Key, baggage.Value);
                }

                foreach(var baggage in data.Baggage)
                {
                    Debug.WriteLine($"{Process.GetCurrentProcess().ProcessName} ENRICHING via Activity.Baggage {baggage.Key}:{baggage.Value}");
                    data.SetTag(baggage.Key, baggage.Value);
                }
            }
        }
    }
}