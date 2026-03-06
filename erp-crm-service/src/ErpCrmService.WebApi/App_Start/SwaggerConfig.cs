using System.Web.Http;
using WebActivatorEx;
using ErpCrmService.WebApi;
using Swashbuckle.Application;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ErpCrmService.WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    // Información básica del API
                    c.SingleApiVersion("v1", "ERP CRM Service API")
                        .Description("API REST para el sistema ERP/CRM - Gestión de Clientes y Productos")
                        .Contact(cc => cc
                            .Name("ERP CRM Team")
                            .Email("support@erp-crm.com")
                            .Url("https://github.com/erp-jc-plataform/erp-crm-service"))
                        .License(lc => lc
                            .Name("MIT License")
                            .Url("https://opensource.org/licenses/MIT"));

                    // Incluir comentarios XML
                    c.IncludeXmlComments(GetXmlCommentsPath());

                    // Esquema personalizado para IDs
                    c.SchemaId(t => t.FullName);

                    // Describir todos los enums como strings
                    c.DescribeAllEnumsAsStrings();

                    // Ignorar propiedades obsoletas
                    c.IgnoreObsoleteProperties();

                    // Ordenar acciones por método HTTP
                    c.GroupActionsBy(apiDesc =>
                    {
                        var controllerName = apiDesc.ActionDescriptor.ControllerDescriptor.ControllerName;
                        return controllerName;
                    });

                    // Soporte para autenticación Bearer (JWT) - Preparado para futuro
                    c.ApiKey("Bearer")
                        .Description("JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"")
                        .Name("Authorization")
                        .In("header");
                })
                .EnableSwaggerUi(c =>
                {
                    // Configuración de la UI de Swagger
                    c.DocumentTitle("ERP CRM Service API Documentation");
                    
                    // Tema oscuro/claro
                    c.InjectStylesheet(thisAssembly, "ErpCrmService.WebApi.SwaggerUI.custom.css");
                    
                    // Habilitar validador de Swagger
                    c.EnableValidator();
                    
                    // Expandir operaciones por defecto
                    c.DocExpansion(DocExpansion.List);
                    
                    // Mostrar duración de peticiones
                    c.DisplayRequestDuration();
                    
                    // Filtro de operaciones
                    c.EnableFilter();
                    
                    // Soporte para OAuth2 (preparado para futuro)
                    c.EnableApiKeySupport("Authorization", "header");
                });
        }

        private static string GetXmlCommentsPath()
        {
            return System.String.Format(@"{0}\bin\ErpCrmService.WebApi.XML",
                System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
