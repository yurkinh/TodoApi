﻿using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TodoApi;

public static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName);

        // TODO: Setup an exporter here
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.SetResourceBuilder(resourceBuilder);
        });

        builder.Services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder)
                   .AddPrometheusExporter()
                   .AddAspNetCoreInstrumentation()
                   .AddRuntimeInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddEventCountersInstrumentation(c =>
                   {
                       // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                       c.AddEventSources(
                           "Microsoft.AspNetCore.Hosting",
                           // There's currently a bug preventing this from working
                           // "Microsoft-AspNetCore-Server-Kestrel"
                           "System.Net.Http", 
                           "System.Net.Sockets",
                           "System.Net.NameResolution",
                           "System.Net.Security");
                   });
        });

        // TODO: Setup an exporter here
        builder.Services.AddOpenTelemetryTracing(tracing =>
        {
            tracing.SetResourceBuilder(resourceBuilder)
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddEntityFrameworkCoreInstrumentation();
        });

        return builder;
    }
}
