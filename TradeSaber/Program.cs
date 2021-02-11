using Auros.Serilog.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using TradeSaber.Services;

namespace TradeSaber
{
    public class Program
    {
        public const string LOG_OUTPUT_FORMAT = "[{Timestamp:HH:mm:ss} | {Level:u3} | {LooseSource}] {Message:lj}{NewLine}{Exception}";
        public const string BAD_LOG = "Route matched with {RouteData}. Executing controller action with signature {MethodInfo} on controller {Controller} ({AssemblyName}).";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((host, services) =>
                {
                    string? path = host.Configuration["TradeSaber:SaveLogsTo"];
                    services.AddSingleton<LoggerProviderCollection>();

                    LoggerConfiguration logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(host.Configuration)
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Information)
                        .WriteTo.Console(outputTemplate: LOG_OUTPUT_FORMAT, theme: AnsiConsoleTheme.Code)
                        .Filter.ByExcluding(log => log.MessageTemplate.Text == BAD_LOG)
                        .Enrich.WithLooseSource()
                        .Enrich.FromLogContext();

                    if (path != null)
                    {
                        if (!Path.IsPathRooted(path))
                            path = Path.Combine(host.HostingEnvironment.ContentRootPath, path);
                        DirectoryInfo directory = new DirectoryInfo(path);
                        directory.Create();
                        if (directory.Exists)
                            logger.WriteTo.File(Path.Combine(path, "log-.txt"), rollingInterval: RollingInterval.Day, outputTemplate: LOG_OUTPUT_FORMAT, rollOnFileSizeLimit: true);
                    }

                    Log.Logger = logger.CreateLogger();
                    services.AddSingleton<ILoggerFactory>(sp =>
                    {
                        LoggerProviderCollection collection = sp.GetRequiredService<LoggerProviderCollection>();
                        SerilogLoggerFactory factory = new SerilogLoggerFactory(dispose: true, providerCollection: collection);
                        foreach (var provider in sp.GetServices<ILoggerProvider>())
                            factory.AddProvider(provider);
                        return factory;
                    });

                    services.AddHostedService<ObjectiveWorker>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}