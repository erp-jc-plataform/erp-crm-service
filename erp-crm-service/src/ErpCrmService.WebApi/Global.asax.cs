using System;
using System.Web;
using System.Web.Http;
using ErpCrmService.WebApi.Infrastructure;
using Serilog;

namespace ErpCrmService.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Configurar Serilog
            LoggerConfig.ConfigureLogger();

            try
            {
                Log.Information("Configuring Web API...");
                GlobalConfiguration.Configure(WebApiConfig.Register);
                Log.Information("Web API configured successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
                throw;
            }
        }

        protected void Application_End()
        {
            LoggerConfig.CloseLogger();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception != null)
            {
                Log.Error(exception, "Unhandled exception in application");
            }
        }
    }
}
