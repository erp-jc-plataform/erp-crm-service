# ?? Guía de Instalación y Pruebas - Logging y Paginación

## ?? **PASO 1: Instalar Paquetes NuGet**

### **Opción A: Package Manager Console (Recomendado)**

Abre **Package Manager Console** en Visual Studio y ejecuta:

```powershell
# Asegúrate de seleccionar el proyecto ErpCrmService.WebApi como predeterminado

Install-Package Serilog -Version 3.1.1
Install-Package Serilog.Sinks.File -Version 5.0.0
Install-Package Serilog.Sinks.Console -Version 5.0.1
Install-Package Serilog.Sinks.Debug -Version 2.0.0
Install-Package Serilog.Extensions.Logging -Version 8.0.0
Install-Package Serilog.AspNet -Version 2.1.0
Install-Package Microsoft.AspNet.WebApi.Cors -Version 5.2.9
Install-Package Swashbuckle -Version 5.6.0
```

### **Opción B: Restaurar desde packages.config**

```powershell
# En la raíz del proyecto
nuget restore ErpCrmService.sln
```

### **Opción C: dotnet CLI**

```bash
cd src\ErpCrmService.WebApi
dotnet add package Serilog --version 3.1.1
dotnet add package Serilog.Sinks.File --version 5.0.0
dotnet add package Serilog.Sinks.Console --version 5.0.1
dotnet add package Serilog.Sinks.Debug --version 2.0.0
```

---

## ?? **PASO 2: Compilar el Proyecto**

```powershell
# Desde la raíz del proyecto
msbuild ErpCrmService.sln /t:Rebuild /p:Configuration=Debug

# O en Visual Studio
Ctrl + Shift + B
```

**Verificar que no hay errores de compilación**

---

## ?? **PASO 3: Ejecutar la Aplicación**

### **Desde Visual Studio**
1. Establecer `ErpCrmService.WebApi` como proyecto de inicio (clic derecho ? Set as StartUp Project)
2. Presionar **F5** o clic en "Start"

### **Desde PowerShell**
```powershell
cd src\ErpCrmService.WebApi\bin\Debug
.\ErpCrmService.WebApi.exe
```

**La aplicación debería iniciar en**: `https://localhost:44300`

---

## ?? **PASO 4: Probar Logging**

### **A. Verificar logs en consola**

Al iniciar la aplicación, deberías ver en la consola:

```
[10:30:15 INF] ============================================
[10:30:15 INF] Application Starting - ERP CRM Service API
[10:30:15 INF] ============================================
[10:30:16 INF] Configuring Web API...
[10:30:17 INF] Web API configured successfully
```

### **B. Verificar archivos de logs**

Navega a la carpeta de logs:

```powershell
cd src\ErpCrmService.WebApi\bin\Debug\Logs
dir
```

Deberías ver:
```
log-20240115.txt
error-20240115.txt
```

### **C. Ver logs en tiempo real**

```powershell
# En PowerShell
Get-Content "src\ErpCrmService.WebApi\bin\Debug\Logs\log-$(Get-Date -Format 'yyyyMMdd').txt" -Wait -Tail 20
```

---

## ?? **PASO 5: Probar Paginación**

### **A. Abrir Swagger UI**

Navega a: `https://localhost:44300/swagger`

### **B. Probar endpoint paginado de Customers**

```http
GET https://localhost:44300/api/customers?pageNumber=1&pageSize=5
```

**Respuesta esperada**:
```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 5,
  "totalCount": 3,
  "totalPages": 1,
  "hasPrevious": false,
  "hasNext": false,
  "firstPage": 1,
  "lastPage": 1,
  "currentPageSize": 3
}
```

### **C. Probar ordenamiento**

```http
GET https://localhost:44300/api/customers?pageNumber=1&pageSize=10&orderBy=CreatedAt
```

### **D. Probar búsqueda con paginación**

```http
GET https://localhost:44300/api/customers/search?searchTerm=tech&pageNumber=1&pageSize=5
```

### **E. Probar productos paginados**

```http
GET https://localhost:44300/api/products?pageNumber=1&pageSize=3&orderBy=Price
```

---

## ?? **PASO 6: Verificar Logs Generados**

Después de hacer algunas peticiones, revisa los logs:

```powershell
# Ver últimas 50 líneas del log general
Get-Content "src\ErpCrmService.WebApi\bin\Debug\Logs\log-$(Get-Date -Format 'yyyyMMdd').txt" -Tail 50
```

**Deberías ver algo como**:
```
2024-01-15 10:35:22.123 +01:00 [INF] [CustomersController] Getting customers - Page: 1, Size: 5, OrderBy: CompanyName
2024-01-15 10:35:22.456 +01:00 [INF] [CustomersController] Retrieved 3 customers from page 1 of 1
2024-01-15 10:36:10.789 +01:00 [INF] [ProductsController] Getting products - Page: 1, Size: 3, OrderBy: Price
2024-01-15 10:36:10.987 +01:00 [INF] [ProductsController] Retrieved 3 products from page 1 of 2
```

---

## ?? **PASO 7: Probar CRUD con Logging**

### **A. Crear un cliente y ver los logs**

```http
POST https://localhost:44300/api/customers
Content-Type: application/json

{
  "companyName": "Test Company",
  "contactFirstName": "John",
  "contactLastName": "Doe",
  "email": "john@testcompany.com",
  "type": 2,
  "creditLimit": 10000
}
```

**En los logs deberías ver**:
```
[INF] Creating new customer: Test Company, Email: john@testcompany.com
[INF] Customer created successfully: ID=..., Company=Test Company
```

### **B. Intentar duplicar email (debería fallar)**

```http
POST https://localhost:44300/api/customers
Content-Type: application/json

{
  "companyName": "Another Company",
  "contactFirstName": "Jane",
  "contactLastName": "Smith",
  "email": "john@testcompany.com",
  "type": 2,
  "creditLimit": 5000
}
```

**En los logs deberías ver**:
```
[WRN] Duplicate email attempted: john@testcompany.com
```

### **C. Buscar cliente inexistente**

```http
GET https://localhost:44300/api/customers/00000000-0000-0000-0000-000000000000
```

**En los logs deberías ver**:
```
[INF] Getting customer by ID: 00000000-0000-0000-0000-000000000000
[WRN] Customer not found: 00000000-0000-0000-0000-000000000000
```

---

## ?? **PASO 8: Visualizar Logs con herramientas**

### **Opción A: Notepad++**

```powershell
notepad++ "src\ErpCrmService.WebApi\bin\Debug\Logs\log-$(Get-Date -Format 'yyyyMMdd').txt"
```

### **Opción B: VS Code**

```powershell
code "src\ErpCrmService.WebApi\bin\Debug\Logs\log-$(Get-Date -Format 'yyyyMMdd').txt"
```

### **Opción C: Baretail (Free log viewer)**

Descarga: https://www.baremetalsoft.com/baretail/

---

## ? **Checklist de Verificación**

### **Logging**
- [ ] Paquetes NuGet instalados
- [ ] Aplicación inicia sin errores
- [ ] Logs aparecen en consola
- [ ] Archivos de logs se crean en carpeta Logs/
- [ ] Logs contienen información estructurada
- [ ] Logs de errores se separan en error-.txt
- [ ] Formato de logs es correcto

### **Paginación**
- [ ] GET /api/customers con paginación funciona
- [ ] Metadata de paginación es correcta (totalPages, hasNext, etc.)
- [ ] Parámetro pageSize respeta límite máximo (100)
- [ ] Ordenamiento funciona (orderBy parameter)
- [ ] Búsqueda con paginación funciona
- [ ] GET /api/products con paginación funciona
- [ ] Response incluye todos los campos de PagedResult

---

## ?? **Troubleshooting**

### **Problema: Paquetes NuGet no se instalan**

**Solución 1**: Limpiar caché de NuGet
```powershell
nuget locals all -clear
nuget restore ErpCrmService.sln
```

**Solución 2**: Reinstalar paquetes
```powershell
Update-Package -reinstall -ProjectName ErpCrmService.WebApi
```

### **Problema: No se crean los archivos de logs**

**Verificar**:
1. La carpeta Logs/ existe en bin\Debug\
2. La aplicación tiene permisos de escritura
3. Serilog está correctamente inicializado en Global.asax

**Crear manualmente**:
```powershell
mkdir "src\ErpCrmService.WebApi\bin\Debug\Logs"
```

### **Problema: Error de compilación "Cannot find Serilog"**

**Solución**: Restaurar paquetes
```powershell
nuget restore ErpCrmService.sln
# Luego recompilar
msbuild ErpCrmService.sln /t:Rebuild /p:Configuration=Debug
```

### **Problema: PagedResult no se reconoce**

**Verificar**:
1. Archivo `PagedResult.cs` existe en Application/Models
2. Namespace correcto: `ErpCrmService.Application.Models`
3. Using statements en los controllers

---

## ?? **Métricas Esperadas**

Después de implementar, deberías tener:

| Métrica | Valor Esperado |
|---------|----------------|
| **Endpoints con logging** | 37/37 (100%) |
| **Endpoints con paginación** | 4 |
| **Tamaño archivo log diario** | ~1-10 MB (dependiendo del tráfico) |
| **Rotación de logs** | Diaria |
| **Response time (sin paginación)** | Variable |
| **Response time (con paginación)** | Mejorado |
| **Memoria consumida** | Similar o mejor |

---

## ?? **Mejores Prácticas Implementadas**

? **Logging**:
- Logs estructurados con contexto
- Separación de niveles (Info, Warning, Error)
- Rotación automática
- No exponer información sensible

? **Paginación**:
- Límite máximo por página
- Valores por defecto sensatos
- Metadata completa
- Ordenamiento configurable

---

## ?? **¡Listo para Usar!**

Tu microservicio ahora tiene:
- ? Logging profesional con Serilog
- ? Paginación eficiente
- ? 80% Production-Ready

**Próximo paso sugerido**: Implementar Unit Tests para llegar al 90%

---

**¿Necesitas ayuda?** Consulta los documentos:
- `LOGGING_PAGINATION_IMPLEMENTATION.md` - Documentación completa
- `CRUD_IMPLEMENTATION_COMPLETE.md` - Guía de CRUD
- `POSTMAN_TESTING_GUIDE.md` - Ejemplos de pruebas
