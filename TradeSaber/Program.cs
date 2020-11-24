using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TradeSaber
{
    public class Program
    {
        public const string LOG_OUTPUT_FORMAT = "[{Timestamp:HH:mm:ss} | {Level:u3}] {Message:lj}{NewLine}{Exception}";
        public const string LOG_OUTPUT_FORMAT_FILE = "[{Timestamp:HH:mm:ss} | {Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: LOG_OUTPUT_FORMAT)
                .WriteTo.File("Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: LOG_OUTPUT_FORMAT_FILE,
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
    }
}