# ?? Guía de Implementación de Swagger en ERP CRM API

## ?? ¿Qué es Swagger?

**Swagger (OpenAPI)** es una especificación para documentar APIs REST de forma interactiva y profesional. Permite:
- ?? Documentación automática de endpoints
- ?? Pruebas interactivas desde el navegador
- ?? Generación de clientes API automáticos
- ? Validación de contratos API

---

## ?? Instalación de Paquetes NuGet

### Opción 1: Package Manager Console
```powershell
# En Visual Studio, abre Package Manager Console y ejecuta:

# Establecer el proyecto WebApi como predeterminado
Install-Package Swashbuckle -Version 5.6.0 -ProjectName ErpCrmService.WebApi
Install-Package Microsoft.AspNet.WebApi -Version 5.2.9 -ProjectName ErpCrmService.WebApi
Install-Package Microsoft.AspNet.WebApi.Cors -Version 5.2.9 -ProjectName ErpCrmService.WebApi
```

### Opción 2: NuGet Package Manager (GUI)
1. Clic derecho en el proyecto `ErpCrmService.WebApi`
2. Selecciona "Manage NuGet Packages"
3. Busca e instala:
   - `Swashbuckle` (v5.6.0)
   - `Microsoft.AspNet.WebApi` (v5.2.9)
   - `Microsoft.AspNet.WebApi.Cors` (v5.2.9)

### Opción 3: Comando dotnet CLI
```bash
cd src\ErpCrmService.WebApi
dotnet add package Swashbuckle --version 5.6.0
dotnet add package Microsoft.AspNet.WebApi --version 5.2.9
dotnet add package Microsoft.AspNet.WebApi.Cors --version 5.2.9
```

---

## ?? Estructura de Archivos Creados

```
src/ErpCrmService.WebApi/
??? App_Start/
?   ??? SwaggerConfig.cs          # ? Configuración de Swagger
?   ??? WebApiConfig.cs            # ? Configuración de Web API
??? Controllers/
?   ??? HomeController.cs          # ? Controlador de inicio
?   ??? CustomersController.cs     # ? API de Clientes
?   ??? ProductsController.cs      # ? API de Productos
??? Global.asax                    # ? Archivo de aplicación
??? Global.asax.cs                 # ? Código de inicio
??? Web.config                     # ??  Necesita configuración manual
```

---

## ?? Configuración del Proyecto

### 1. Habilitar generación de documentación XML

**Opción A: Desde Visual Studio**
1. Clic derecho en `ErpCrmService.WebApi` ? Properties
2. Ve a la pestaña "Build"
3. Marca "XML documentation file"
4. Ruta: `bin\ErpCrmService.WebApi.XML`
5. Guardar

**Opción B: Editar .csproj manualmente**
Agrega esto dentro de `<PropertyGroup>` de Debug y Release:
```xml
<DocumentationFile>bin\ErpCrmService.WebApi.XML</DocumentationFile>
```

### 2. Agregar referencias al proyecto Infrastructure

En `ErpCrmService.WebApi.csproj`, agregar:
```xml
<ItemGroup>
  <ProjectReference Include="..\ErpCrmService.Infrastructure\ErpCrmService.Infrastructure.csproj">
    <Project>{C3D4E5F6-A7B8-4C5D-8E7F-9A0B1C2D3E4F}</Project>
    <Name>ErpCrmService.Infrastructure</Name>
  </ProjectReference>
  <ProjectReference Include="..\ErpCrmService.Domain\ErpCrmService.Domain.csproj">
    <Project>{A1B2C3D4-E5F6-7890-1234-567890ABCDEF}</Project>
    <Name>ErpCrmService.Domain</Name>
  </ProjectReference>
</ItemGroup>
```

---

## ?? Configuración de Web.config

Si el archivo `Web.config` no se creó correctamente, créalo manualmente con este contenido:

<details>
<summary>Ver Web.config completo</summary>

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=DESKTOP-40FEK5D\MSSQLSERVERJC;Initial Catalog=CRM;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <system.web>
    <compilation debug="true" targetFramework="4.8" />
    <httpRuntime targetFramework="4.8" />
  </system.web>
  
  <system.webServer>
    <handlers>
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
    </handlers>
  </system.webServer>
  
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" />
        <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Http" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-5.2.9.0" newVersion="5.2.9.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
  
  <entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.SqlConnectionFactory, EntityFramework" />
    <providers>
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
    </providers>
  </entityFramework>
</configuration>
```
</details>

---

## ?? Ejecutar el Proyecto

### 1. Compilar la Solución
```powershell
# Desde la raíz del proyecto
msbuild ErpCrmService.sln /t:Rebuild /p:Configuration=Debug
```

O en Visual Studio: **Ctrl + Shift + B**

### 2. Ejecutar el Proyecto
1. Establecer `ErpCrmService.WebApi` como proyecto de inicio (clic derecho ? Set as StartUp Project)
2. Presionar **F5** o clic en "Start"

### 3. Acceder a Swagger
Una vez iniciado, abre tu navegador en:
- **Swagger UI**: `https://localhost:44300/swagger`
- **API Info**: `https://localhost:44300/api`
- **Health Check**: `https://localhost:44300/api/health`

---

## ?? Endpoints Disponibles

### ?? Home
- `GET /api` - Información del API
- `GET /api/health` - Estado del servicio

### ?? Customers (Clientes)
- `GET /api/customers` - Todos los clientes
- `GET /api/customers/{id}` - Cliente por ID
- `GET /api/customers/search?searchTerm={term}` - Buscar clientes
- `GET /api/customers/status/{status}` - Clientes por estado
- `GET /api/customers/balance-summary` - Resumen de balances
- `GET /api/customers/statistics` - Estadísticas de clientes

### ?? Products (Productos)
- `GET /api/products` - Todos los productos
- `GET /api/products/{id}` - Producto por ID
- `GET /api/products/sku/{sku}` - Producto por SKU
- `GET /api/products/search?searchTerm={term}` - Buscar productos
- `GET /api/products/category/{category}` - Productos por categoría
- `GET /api/products/low-stock` - Productos con stock bajo
- `GET /api/products/discontinued` - Productos descontinuados
- `GET /api/products/inventory-stats` - Estadísticas de inventario
- `GET /api/products/top-value?top=10` - Top productos por valor

---

## ?? Personalización de Swagger

### Cambiar Título y Descripción
Edita `SwaggerConfig.cs`:
```csharp
c.SingleApiVersion("v1", "Tu Título Personalizado")
    .Description("Tu descripción personalizada")
```

### Agregar Autenticación Bearer
Ya está configurado en `SwaggerConfig.cs`. Para usarlo:
1. En Swagger UI, clic en el botón "Authorize"
2. Ingresa: `Bearer {tu-token-jwt}`

### Personalizar Tema
Crea un archivo CSS en `SwaggerUI\custom.css` y regístralo en `SwaggerConfig.cs`:
```csharp
c.InjectStylesheet(thisAssembly, "ErpCrmService.WebApi.SwaggerUI.custom.css");
```

---

## ? Verificación de la Instalación

### Checklist
- [ ] Paquetes NuGet instalados
- [ ] Archivos de configuración creados
- [ ] Referencias de proyecto agregadas
- [ ] XML documentation habilitada
- [ ] Proyecto compila sin errores
- [ ] Swagger accesible en `/swagger`
- [ ] Endpoints responden correctamente

### Comandos de Verificación
```powershell
# Verificar que Swagger está configurado
Test-Path "src\ErpCrmService.WebApi\App_Start\SwaggerConfig.cs"

# Verificar controladores
Get-ChildItem "src\ErpCrmService.WebApi\Controllers" -Filter "*.cs"

# Compilar
msbuild src\ErpCrmService.WebApi\ErpCrmService.WebApi.csproj /t:Build /p:Configuration=Debug
```

---

## ?? Troubleshooting

### Problema: Swagger no se muestra
**Solución:**
1. Verifica que `SwaggerConfig.cs` existe en `App_Start`
2. Verifica que `WebActivatorEx` está instalado
3. Limpia y recompila la solución

### Problema: Errores de compilación
**Solución:**
1. Restaura paquetes NuGet: `nuget restore`
2. Verifica referencias de proyectos
3. Limpia solución: `Clean Solution` ? `Rebuild Solution`

### Problema: No encuentra DbContext
**Solución:**
1. Agrega referencia a `ErpCrmService.Infrastructure`
2. Verifica que el connection string está en `Web.config`

### Problema: XML Comments no aparecen
**Solución:**
1. Habilita "XML documentation file" en propiedades del proyecto
2. Recompila el proyecto
3. Verifica que el archivo XML se genera en `bin\`

---

## ?? Mejores Prácticas

### Documentación de Controladores
```csharp
/// <summary>
/// Descripción clara del endpoint
/// </summary>
/// <param name="id">Descripción del parámetro</param>
/// <returns>Descripción de lo que retorna</returns>
/// <response code="200">Caso exitoso</response>
/// <response code="404">Caso no encontrado</response>
[HttpGet]
[Route("{id}")]
[ResponseType(typeof(MiEntidad))]
public IHttpActionResult Get(Guid id)
{
    // ...
}
```

### Versionado de API
Para futuras versiones, considera:
- URL: `/api/v2/customers`
- Header: `api-version: 2.0`
- Query: `/api/customers?api-version=2.0`

### Seguridad
- Implementa autenticación JWT
- Usa HTTPS en producción
- Valida todos los inputs
- Maneja excepciones correctamente

---

## ?? Recursos Adicionales

- [Documentación de Swashbuckle](https://github.com/domaindrivendev/Swashbuckle)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Web API](https://docs.microsoft.com/en-us/aspnet/web-api/)

---

**¡Tu API ahora es más profesional con Swagger! ??**

*Fecha de creación: $(Get-Date -Format "dd/MM/yyyy")*
