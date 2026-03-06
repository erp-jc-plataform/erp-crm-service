using System;
using System.IO;
using Serilog;
using Serilog.Events;

namespace ErpCrmService.WebApi.Infrastructure
{
    /// <summary>
    /// Configuración centralizada de Serilog para la aplicación
    /// </summary>
    public static class LoggerConfig
    {
        /// <summary>
        /// Configura Serilog con múltiples sinks
        /// </summary>
        public static void ConfigureLogger()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Debug()
                .WriteTo.File(
                    path: Path.Combine(logPath, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    fileSizeLimitBytes: 10485760) // 10MB
                .WriteTo.File(
                    path: Path.Combine(logPath, "error-.txt"),
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    retainedFileCountLimit: 90,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    fileSizeLimitBytes: 10485760)
                .CreateLogger();

            Log.Information("============================================");
            Log.Information("Application Starting - ERP CRM Service API");
            Log.Information("============================================");
        }

        /// <summary>
        /// Cierra y libera los recursos del logger
        /// </summary>
        public static void CloseLogger()
        {
            Log.Information("============================================");
            Log.Information("Application Stopping - ERP CRM Service API");
            Log.Information("============================================");
            Log.CloseAndFlush();
        }
    }
}
