using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common
{
    public static class OpenTelemetryConfiguration
    {
        public static readonly ActivitySource ActivitySource = new ActivitySource("OpenTelemetrySample");

        public static void ConfigureOpenTelemetry(this IServiceCollection services, string applicationName)
        {
            services.AddOpenTelemetryTracing(t =>
                t.AddSource(ActivitySource.Name)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(applicationName))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddGenevaTraceExporter(g =>
                {
                    g.ConnectionString = "EtwSession=OpenTelemetry";
                    g.PrepopulatedFields = new Dictionary<string, object> { { "pid", Environment.ProcessId } };
                })
                .AddOtlpExporter(o =>
                {                    
                    o.Endpoint = new Uri("http://localhost:4317");
                    o.ExportProcessorType = ExportProcessorType.Simple;
                })
            );
        }
    }
}