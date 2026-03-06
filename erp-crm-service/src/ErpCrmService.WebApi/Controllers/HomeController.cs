using System.Web.Http;

namespace ErpCrmService.WebApi.Controllers
{
    /// <summary>
    /// Controlador principal del API
    /// </summary>
    public class HomeController : ApiController
    {
        /// <summary>
        /// Página de inicio del API - Redirige a Swagger
        /// </summary>
        /// <returns>Información del API</returns>
        [HttpGet]
        [Route("")]
        [Route("api")]
        public IHttpActionResult Get()
        {
            var apiInfo = new
            {
                Name = "ERP CRM Service API",
                Version = "v1.0",
                Description = "API REST para gestión de clientes y productos",
                Documentation = "/swagger",
                Endpoints = new
                {
                    Customers = "/api/customers",
                    Products = "/api/products",
                    Swagger = "/swagger",
                    SwaggerJson = "/swagger/docs/v1"
                },
                Status = "Online",
                Database = "CRM",
                Server = "DESKTOP-40FEK5D\\MSSQLSERVERJC"
            };

            return Ok(apiInfo);
        }

        /// <summary>
        /// Verifica el estado del API
        /// </summary>
        /// <returns>Estado del servicio</returns>
        [HttpGet]
        [Route("api/health")]
        public IHttpActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = System.DateTime.UtcNow,
                Service = "ERP CRM API",
                Version = "1.0.0"
            });
        }
    }
}
