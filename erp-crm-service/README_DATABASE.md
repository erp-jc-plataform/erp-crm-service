# ERP CRM Service - Guía de Configuración de Base de Datos

## ?? Información General

Este proyecto es un sistema ERP/CRM construido con:
- **.NET Framework 4.8**
- **Entity Framework 6**
- **SQL Server** (MSSQLSERVERJC)
- **Arquitectura en Capas** (Domain, Application, Infrastructure, WebApi)

---

## ??? Configuración de Base de Datos

### Servidor Configurado
```
Servidor: DESKTOP-40FEK5D\MSSQLSERVERJC
Base de Datos: CRM
Autenticación: Windows Authentication
```

### Connection String
La configuración se encuentra en: `src\ErpCrmService.Infrastructure\App.config`

```xml
<connectionStrings>
  <add name="DefaultConnection" 
       connectionString="Data Source=DESKTOP-40FEK5D\MSSQLSERVERJC;
                        Initial Catalog=CRM;
                        Integrated Security=True;
                        Connect Timeout=30;
                        Encrypt=False;
                        TrustServerCertificate=False;
                        ApplicationIntent=ReadWrite;
                        MultiSubnetFailover=False" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## ?? Instalación y Configuración Inicial

### 1. Prerrequisitos
- Visual Studio 2019 o superior
- SQL Server (Express/Developer/Standard)
- .NET Framework 4.8 SDK

### 2. Restaurar Paquetes NuGet
```powershell
# En la raíz del proyecto
dotnet restore
```

O desde Visual Studio:
- Clic derecho en la solución ? "Restore NuGet Packages"

### 3. Crear Base de Datos

#### Opción A: Usando SQL Script (Recomendado)
```powershell
# Ejecutar el script de creación
sqlcmd -S "DESKTOP-40FEK5D\MSSQLSERVERJC" -d CRM -E -i CreateDatabaseTables.sql
```

#### Opción B: Usando Entity Framework Migrations
```powershell
# En Package Manager Console
Enable-Migrations -ContextTypeName ErpCrmDbContext
Add-Migration InitialCreate
Update-Database -Verbose
```

### 4. Verificar Configuración
```powershell
# Ejecutar script de prueba
.\TestDatabaseConnection.ps1
```

**Resultado esperado:**
```
? Conexión exitosa!
? Todas las tablas necesarias están presentes.
  Customers : X registros
  Products : X registros
```

---

## ?? Esquema de Base de Datos

### Tabla: Customers (22 columnas)
Almacena información de clientes del sistema CRM.

**Campos principales:**
- `Id` (GUID, PK)
- `CompanyName`, `ContactFirstName`, `ContactLastName`
- `Email` (Único), `Phone`
- `Address` (Street, City, State, PostalCode, Country)
- `TaxId` (Único)
- `Status` (Active, Inactive, Suspended, Blocked)
- `Type` (Individual, Corporate, Government, NonProfit)
- `CreditLimit`, `CurrentBalance`
- Auditoría: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `IsActive`

**Índices:**
- `IX_Customer_Email` (Único)
- `IX_Customer_TaxId` (Único, filtrado)
- `IX_Customer_CompanyName`
- `IX_Customer_Status_Active`

### Tabla: Products (23 columnas)
Gestiona el catálogo de productos.

**Campos principales:**
- `Id` (GUID, PK)
- `SKU` (Único), `Name`, `Description`
- `Category`, `UnitPrice`, `Cost`
- `StockQuantity`, `MinimumStock`, `MaximumStock`, `Unit`
- `IsDiscontinued`, `DiscontinuedDate`
- `Supplier`
- Dimensiones: `Weight`, `Length`, `Width`, `Height`
- Auditoría: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `IsActive`

**Índices:**
- `IX_Product_SKU` (Único)
- `IX_Product_Name`
- `IX_Product_Category_Active`
- `IX_Product_Stock`

---

## ??? Scripts Disponibles

| Script | Descripción |
|--------|-------------|
| `CreateDatabaseTables.sql` | Crea las tablas e índices en la base de datos |
| `InsertTestData.sql` | Inserta datos de prueba (3 clientes, 5 productos) |
| `TestDatabaseConnection.ps1` | Verifica la conexión y estado de las tablas |
| `ExecuteCreateTables.ps1` | Ejecuta automáticamente la creación de tablas |
| `InitializeDatabase.ps1` | Script de inicialización completo |

---

## ?? Uso de la Aplicación

### Compilar el Proyecto
```powershell
# Desde la línea de comandos
msbuild ErpCrmService.sln /t:Rebuild /p:Configuration=Release

# O presiona F6 en Visual Studio
```

### Ejecutar la Web API
```powershell
# Establecer ErpCrmService.WebApi como proyecto de inicio
# Presionar F5 en Visual Studio
```

---

## ?? Solución de Problemas

### Error: "Cannot open database CRM"
**Solución:**
1. Verificar que SQL Server esté corriendo
2. Comprobar el nombre del servidor en el connection string
3. Asegurarse de tener permisos en la base de datos

```powershell
# Probar conexión
sqlcmd -S "DESKTOP-40FEK5D\MSSQLSERVERJC" -E -Q "SELECT @@SERVERNAME"
```

### Error: "Login failed for user"
**Solución:**
1. Verificar Windows Authentication en SQL Server
2. Agregar el usuario actual a SQL Server:
```sql
USE [master]
GO
CREATE LOGIN [DOMAIN\Username] FROM WINDOWS
GO
USE [CRM]
GO
CREATE USER [DOMAIN\Username] FOR LOGIN [DOMAIN\Username]
GO
ALTER ROLE [db_owner] ADD MEMBER [DOMAIN\Username]
GO
```

### Error: NuGet packages missing
**Solución:**
```powershell
# Restaurar paquetes
nuget restore ErpCrmService.sln
```

---

## ?? Datos de Prueba

### Clientes Insertados
1. **TechSolutions SA** - Madrid (Corporativo) - €50,000 límite
2. **Retail Express** - Barcelona (Corporativo) - €75,000 límite
3. **Consultora García** - Sevilla (Individual) - €10,000 límite

### Productos Insertados
1. **LAP-001** - Laptop Dell XPS 15 - €1,499.99
2. **MON-001** - Monitor LG 27" 4K - €399.99
3. **KEY-001** - Teclado Mecánico RGB - €89.99
4. **MOU-001** - Mouse MX Master 3 - €99.99
5. **PRI-001** - Impresora HP LaserJet - €249.99 ?? **Stock Bajo**

---

## ??? Arquitectura del Proyecto

```
ErpCrmService/
??? src/
?   ??? ErpCrmService.Domain/          # Entidades y lógica de negocio
?   ??? ErpCrmService.Application/     # Casos de uso y DTOs
?   ??? ErpCrmService.Infrastructure/  # Acceso a datos (EF, Repos)
?   ??? ErpCrmService.WebApi/          # API REST endpoints
??? CreateDatabaseTables.sql           # Script de creación de BD
??? InsertTestData.sql                 # Datos de prueba
??? TestDatabaseConnection.ps1         # Script de verificación
??? DATABASE_SETUP_SUMMARY.md          # Documentación detallada
```

---

## ?? Tecnologías Utilizadas

- **.NET Framework 4.8**
- **Entity Framework 6**
- **SQL Server 2019**
- **ASP.NET Web API 2**
- **Patrón Repository**
- **Domain-Driven Design (DDD)**
- **Value Objects** (Email, Phone, Address)

---

## ?? Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

---

## ?? Licencia

Este proyecto es privado y confidencial.

---

## ?? Soporte

Para soporte técnico, contacta al equipo de desarrollo.

---

**Última actualización:** $(Get-Date -Format "dd/MM/yyyy")
**Estado del Proyecto:** ?? Activo - En Desarrollo
