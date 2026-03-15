# erp-crm-service

Microservicio de gestion de **Clientes y Productos** para Business ERP, desarrollado con ASP.NET Web API 2 y .NET Framework 4.8. Implementa arquitectura limpia (Clean Architecture) con cuatro capas independientes y expone una API REST documentada con Swagger (Swashbuckle).

---

## Lenguaje y Stack Tecnologico

| Capa | Tecnologia | Version |
|------|-----------|---------|
| Lenguaje | **C#** | .NET Framework 4.8 |
| Framework web | **ASP.NET Web API 2** | 5.2.7 |
| ORM | **Entity Framework** | 6.4.4 |
| Serializacion JSON | **Newtonsoft.Json** | 12.0.2 |
| Logging | **Serilog** | - |
| Documentacion API | **Swashbuckle** (Swagger) | - |
| Base de datos | **SQL Server** | >= 2017 |
| Servidor de desarrollo | **IIS Express** | incluido con Visual Studio |

---

## Arquitectura

El servicio sigue el patron de arquitectura limpia con cuatro proyectos:

```
src/
├── ErpCrmService.Domain/         # Entidades, Value Objects, contratos
├── ErpCrmService.Application/    # DTOs, Servicios, Interfaces, Mappers
├── ErpCrmService.Infrastructure/ # DbContext (EF6), Repositorios
└── ErpCrmService.WebApi/         # Controllers, Swagger, configuracion
```

### Dependencias entre capas

```
WebApi --> Application --> Domain
WebApi --> Infrastructure --> Domain
```

---

## Estructura del Proyecto

```
Business-CRM/
└── erp-crm-service/
    ├── src/
    │   ├── ErpCrmService.sln                    # Solucion Visual Studio
    │   ├── ErpCrmService.Domain/
    │   │   ├── Entities/
    │   │   │   ├── Customer.cs                  # Entidad cliente
    │   │   │   └── Product.cs                   # Entidad producto
    │   │   ├── ValueObjects/                    # Email, PhoneNumber, Address
    │   │   ├── Repositories/                    # Interfaces de repositorios
    │   │   └── Services/                        # Servicios de dominio
    │   ├── ErpCrmService.Application/
    │   │   ├── DTOs/                            # Objetos de transferencia de datos
    │   │   ├── Services/                        # Logica de aplicacion
    │   │   ├── Interfaces/                      # Contratos de servicios
    │   │   ├── Mappers/                         # Mapeo DTO <-> Entidad
    │   │   ├── Models/                          # Modelos auxiliares (PagedResult, etc.)
    │   │   └── Extensions/                      # Extension methods (paginacion)
    │   ├── ErpCrmService.Infrastructure/
    │   │   ├── Data/
    │   │   │   └── ErpCrmDbContext.cs            # DbContext Entity Framework 6
    │   │   ├── Repositories/                    # Implementacion de repositorios
    │   │   └── App.config                       # Connection string SQL Server
    │   └── ErpCrmService.WebApi/
    │       ├── Controllers/
    │       │   ├── CustomersController.cs        # 12 endpoints para clientes
    │       │   └── ProductsController.cs         # 16 endpoints para productos
    │       ├── App_Start/
    │       │   ├── SwaggerConfig.cs             # Configuracion Swashbuckle
    │       │   └── WebApiConfig.cs              # Rutas, CORS, JSON formatter
    │       ├── Infrastructure/
    │       │   └── LoggerConfig.cs              # Configuracion Serilog
    │       └── Global.asax.cs                   # Inicializacion de la app
    ├── CreateDatabaseTables.sql                 # Script creacion de tablas
    ├── InsertTestData.sql                       # Script datos de prueba
    └── InitializeDatabase.ps1                  # Script PowerShell para BD
```

---

## Requisitos Previos

- **Visual Studio 2019 o 2022** (con workload de desarrollo web ASP.NET)
- **.NET Framework 4.8** (incluido con VS o instalable por separado)
- **SQL Server** (Local, Express o Developer) — recomendado SQL Server 2017+
- **SQL Server Management Studio** (SSMS) o equivalente — opcional

---

## Instalacion

### 1. Abrir la solucion

```
Visual Studio → Abrir archivo → seleccionar:
C:\Proyectos\BusinessApp\Business-CRM\erp-crm-service\src\ErpCrmService.sln
```

### 2. Restaurar paquetes NuGet

```
Visual Studio → click derecho en la solucion → Restore NuGet Packages
```

O desde la consola de paquetes (Tools → NuGet Package Manager → Package Manager Console):

```powershell
Update-Package -Reinstall
```

### 3. Configurar la cadena de conexion

Editar `src\ErpCrmService.Infrastructure\App.config`:

```xml
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=TU_SERVIDOR\INSTANCIA;Initial Catalog=CRM;Integrated Security=True;Connect Timeout=30;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Reemplazar `TU_SERVIDOR\INSTANCIA` con el nombre de tu instancia de SQL Server.
Para SQL Server local con autenticacion de Windows: `Data Source=.\SQLEXPRESS` o `Data Source=(localdb)\MSSQLLocalDB`.

### 4. Crear la base de datos

**Opcion A: Desde SSMS**

1. Abrir SSMS y conectarse a tu instancia
2. Crear base de datos `CRM`:
   ```sql
   CREATE DATABASE CRM;
   ```
3. Ejecutar el script de tablas:
   ```sql
   -- Abrir y ejecutar: CreateDatabaseTables.sql
   ```
4. (Opcional) Insertar datos de prueba:
   ```sql
   -- Abrir y ejecutar: InsertTestData.sql
   ```

**Opcion B: Desde PowerShell**

```powershell
cd C:\Proyectos\BusinessApp\Business-CRM\erp-crm-service
.\InitializeDatabase.ps1
```

---

## Levantar el Microservicio

### Desde Visual Studio (recomendado)

1. Abrir `src\ErpCrmService.sln` en Visual Studio
2. Establecer `ErpCrmService.WebApi` como proyecto de inicio (click derecho → "Set as Startup Project")
3. Presionar **F5** (con debugger) o **Ctrl+F5** (sin debugger)
4. IIS Express arranca automaticamente

El servidor estara disponible en:
- HTTPS: `https://localhost:44300`
- HTTP: `http://localhost:61234`

### Desde linea de comandos con MSBuild

```powershell
cd C:\Proyectos\BusinessApp\Business-CRM\erp-crm-service\src

# Compilar en Release
msbuild ErpCrmService.sln /p:Configuration=Release

# Iniciar con IIS Express (requiere IIS Express instalado)
"C:\Program Files\IIS Express\iisexpress.exe" /path:"ruta_absoluta_a_WebApi" /port:61234
```

---

## URLs Disponibles

| URL | Descripcion |
|-----|-------------|
| `https://localhost:44300/swagger` | Swagger UI — documentacion interactiva |
| `https://localhost:44300/swagger/docs/v1` | Spec JSON de la API |
| `https://localhost:44300/api/customers` | Endpoint de clientes |
| `https://localhost:44300/api/products` | Endpoint de productos |

---

## Endpoints de la API

### Clientes (`/api/customers`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/customers` | Listar clientes activos (paginado, ordenable) |
| GET | `/api/customers/{id}` | Obtener cliente por GUID |
| GET | `/api/customers/search` | Buscar clientes por texto |
| GET | `/api/customers/status/{status}` | Filtrar por estado |
| GET | `/api/customers/balance-summary` | Resumen de balances y creditos |
| GET | `/api/customers/statistics` | Estadisticas generales de clientes |
| POST | `/api/customers` | Crear nuevo cliente |
| PUT | `/api/customers/{id}` | Actualizar cliente completo |
| PATCH | `/api/customers/{id}/status` | Cambiar estado del cliente |
| PATCH | `/api/customers/{id}/balance` | Actualizar balance del cliente |
| DELETE | `/api/customers/{id}` | Eliminar cliente (soft delete) |
| POST | `/api/customers/{id}/restore` | Restaurar cliente eliminado |

#### Parametros de paginacion (GET /api/customers)

| Parametro | Tipo | Default | Descripcion |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Numero de pagina |
| `pageSize` | int | 20 | Registros por pagina (max 100) |
| `orderBy` | string | CompanyName | Campo de ordenamiento |

### Productos (`/api/products`)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | `/api/products` | Listar productos activos (paginado, ordenable) |
| GET | `/api/products/{id}` | Obtener producto por GUID |
| GET | `/api/products/sku/{sku}` | Buscar producto por SKU |
| GET | `/api/products/search` | Buscar productos por texto |
| GET | `/api/products/category/{category}` | Filtrar por categoria |
| GET | `/api/products/low-stock` | Productos con stock bajo |
| GET | `/api/products/discontinued` | Productos descontinuados |
| GET | `/api/products/inventory-stats` | Estadisticas de inventario |
| GET | `/api/products/top-value` | Productos de mayor valor |
| POST | `/api/products` | Crear nuevo producto |
| PUT | `/api/products/{id}` | Actualizar producto completo |
| PATCH | `/api/products/{id}/stock` | Actualizar stock del producto |
| PATCH | `/api/products/{id}/pricing` | Actualizar precio del producto |
| POST | `/api/products/{id}/discontinue` | Marcar producto como descontinuado |
| DELETE | `/api/products/{id}` | Eliminar producto (soft delete) |
| POST | `/api/products/{id}/restore` | Restaurar producto eliminado |

---

## Modelo de Datos

### Tabla `Customers`

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `Id` | uniqueidentifier | Clave primaria (GUID) |
| `CompanyName` | nvarchar(100) | Nombre de la empresa |
| `ContactFirstName` | nvarchar(50) | Nombre del contacto |
| `ContactLastName` | nvarchar(50) | Apellido del contacto |
| `Email` | nvarchar(150) | Correo electronico |
| `Phone` | nvarchar(20) | Telefono |
| `AddressStreet/City/State/PostalCode/Country` | nvarchar | Direccion completa |
| `TaxId` | nvarchar(20) | RUC / NIT / Numero fiscal |
| `Status` | int | Estado del cliente (enum) |
| `Type` | int | Tipo de cliente (enum) |
| `CreditLimit` | decimal(18,2) | Limite de credito |
| `CurrentBalance` | decimal(18,2) | Balance actual |
| `IsActive` | bit | Soft delete |
| `CreatedAt` | datetime | Fecha de creacion |
| `UpdatedAt` | datetime | Ultima modificacion |

### Tabla `Products`

| Campo | Tipo | Descripcion |
|-------|------|-------------|
| `Id` | uniqueidentifier | Clave primaria (GUID) |
| `Name` | nvarchar | Nombre del producto |
| `SKU` | nvarchar | Codigo unico de producto |
| `Price` | decimal(18,2) | Precio unitario |
| `Stock` | int | Cantidad en inventario |
| `Category` | int | Categoria (enum) |
| `IsDiscontinued` | bit | Producto descontinuado |
| `IsActive` | bit | Soft delete |
| `CreatedAt` | datetime | Fecha de creacion |

---

## Swagger

Swashbuckle genera la documentacion automaticamente a partir de los XML comments de los controllers y los schemas de las entidades.

Para acceder:
1. Levantar el proyecto desde Visual Studio (F5)
2. Abrir `https://localhost:44300/swagger`
3. Explorar y probar los endpoints directamente desde la interfaz

La especificacion JSON esta disponible en `https://localhost:44300/swagger/docs/v1` para importar en Postman o cualquier cliente REST.

---

## CORS

CORS esta habilitado para todos los origenes en modo desarrollo (`"*"`).
Para produccion, ajustar en `App_Start\WebApiConfig.cs`:

```csharp
var cors = new EnableCorsAttribute("https://mi-frontend.com", "*", "*");
```

---

## Logs

Serilog esta configurado en `Infrastructure\LoggerConfig.cs`. Cada operacion de los controllers genera logs con nivel y contexto apropiado usando `Log.ForContext<NombreController>()`.

---

## Configuracion de Produccion

1. Cambiar la cadena de conexion en `App.config` a la instancia de SQL Server de produccion
2. Compilar en modo `Release`
3. Publicar en IIS con Web Deploy o copiando el output de `bin\Release`
4. Configurar CORS restringido a los dominios del frontend
5. Deshabilitar la pagina de Swagger en produccion (comentar el registro en `SwaggerConfig.cs`)

---

## Licencia

Proyecto interno — Business ERP.
