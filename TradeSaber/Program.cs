using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.SystemConsole.Themes;

namespace TradeSaber
{
    public class Program
    {
        public const string LOG_OUTPUT_FORMAT = "[{Timestamp:HH:mm:ss} | {Level:u3}] {LooseSource} {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.With<LooseSourceEnricher>()
                .WriteTo.Console(outputTemplate: LOG_OUTPUT_FORMAT, theme: AnsiConsoleTheme.Code)
                .WriteTo.File("Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LOG_OUTPUT_FORMAT,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public class LooseSourceEnricher : ILogEventEnricher
        {
            private readonly Dictionary<string, LogEventProperty> _propertyCache = new Dictionary<string, LogEventProperty>();
            private static readonly string looseSourceName = "LooseSource";
            private static readonly string sourceName = "SourceContext";

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (logEvent.Properties.TryGetValue(sourceName, out LogEventPropertyValue? value) && value != null)
                {
                    var subs = value.ToString().Split('.');
                    if (subs.Length > 0)
                    {
                        var sub = $"[{subs[^1].Replace("\"", "")}]";
                        if (!_propertyCache.TryGetValue(sub, out LogEventProperty? property))
                        {
                            property = propertyFactory.CreateProperty(looseSourceName, sub);
                            _propertyCache.TryAdd(sub, property);
                        }
                        logEvent.AddPropertyIfAbsent(property);
                    }
                }
            }
        }
    }
}