# Script para inicializar la base de datos usando Entity Framework
# Este script genera las tablas automáticamente usando el DbContext

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Inicialización de Base de Datos CRM" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "OPCIONES PARA CREAR LAS TABLAS:" -ForegroundColor Yellow
Write-Host ""

Write-Host "OPCIÓN 1: Usar Code First (Recomendado)" -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Green
Write-Host "1. Abre Visual Studio" -ForegroundColor White
Write-Host "2. Ve a: Tools > NuGet Package Manager > Package Manager Console" -ForegroundColor White
Write-Host "3. Asegúrate de que el Default Project sea: ErpCrmService.Infrastructure" -ForegroundColor White
Write-Host "4. Ejecuta los siguientes comandos:" -ForegroundColor White
Write-Host ""
Write-Host "   Enable-Migrations -ContextTypeName ErpCrmDbContext" -ForegroundColor Cyan
Write-Host "   Add-Migration InitialCreate" -ForegroundColor Cyan
Write-Host "   Update-Database -Verbose" -ForegroundColor Cyan
Write-Host ""

Write-Host "OPCIÓN 2: Crear tablas manualmente con SQL" -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Green
Write-Host "Ejecuta el script SQL generado a continuación en SQL Server Management Studio" -ForegroundColor White
Write-Host ""

# Generar script SQL para crear las tablas
$sqlScript = @"
-- Script de creación de tablas para CRM Database
-- Generado para: DESKTOP-40FEK5D\MSSQLSERVERJC
-- Base de datos: CRM

USE [CRM];
GO

-- Tabla Customers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customers](
        [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
        [CompanyName] [nvarchar](100) NOT NULL,
        [ContactFirstName] [nvarchar](50) NOT NULL,
        [ContactLastName] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](150) NOT NULL,
        [Phone] [nvarchar](20) NULL,
        [AddressStreet] [nvarchar](200) NULL,
        [AddressCity] [nvarchar](100) NULL,
        [AddressState] [nvarchar](100) NULL,
        [AddressPostalCode] [nvarchar](20) NULL,
        [AddressCountry] [nvarchar](100) NULL,
        [TaxId] [nvarchar](20) NULL,
        [Status] [int] NOT NULL,
        [Type] [int] NOT NULL,
        [Notes] [nvarchar](500) NULL,
        [CreditLimit] [decimal](18, 2) NOT NULL DEFAULT 0,
        [CurrentBalance] [decimal](18, 2) NOT NULL DEFAULT 0,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        [CreatedBy] [nvarchar](100) NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1
    );

    -- Índices para Customers
    CREATE UNIQUE INDEX [IX_Customer_Email] ON [dbo].[Customers]([Email]);
    CREATE UNIQUE INDEX [IX_Customer_TaxId] ON [dbo].[Customers]([TaxId]) WHERE [TaxId] IS NOT NULL;
    CREATE INDEX [IX_Customer_CompanyName] ON [dbo].[Customers]([CompanyName]);
    CREATE INDEX [IX_Customer_Status_Active] ON [dbo].[Customers]([Status], [IsActive]);

    PRINT '? Tabla Customers creada exitosamente';
END
ELSE
BEGIN
    PRINT '? Tabla Customers ya existe';
END
GO

-- Tabla Products
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products](
        [Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
        [SKU] [nvarchar](20) NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Category] [int] NOT NULL,
        [UnitPrice] [decimal](18, 2) NOT NULL DEFAULT 0,
        [Cost] [decimal](18, 2) NOT NULL DEFAULT 0,
        [StockQuantity] [int] NOT NULL DEFAULT 0,
        [MinimumStock] [int] NOT NULL DEFAULT 0,
        [MaximumStock] [int] NOT NULL DEFAULT 0,
        [Unit] [nvarchar](50) NOT NULL,
        [IsDiscontinued] [bit] NOT NULL DEFAULT 0,
        [DiscontinuedDate] [datetime] NULL,
        [Supplier] [nvarchar](100) NULL,
        [Weight] [decimal](18, 2) NULL,
        [Length] [decimal](18, 2) NULL,
        [Width] [decimal](18, 2) NULL,
        [Height] [decimal](18, 2) NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime] NULL,
        [CreatedBy] [nvarchar](100) NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1
    );

    -- Índices para Products
    CREATE UNIQUE INDEX [IX_Product_SKU] ON [dbo].[Products]([SKU]);
    CREATE INDEX [IX_Product_Name] ON [dbo].[Products]([Name]);
    CREATE INDEX [IX_Product_Category_Active] ON [dbo].[Products]([Category], [IsActive]);
    CREATE INDEX [IX_Product_Stock] ON [dbo].[Products]([StockQuantity], [MinimumStock]);

    PRINT '? Tabla Products creada exitosamente';
END
ELSE
BEGIN
    PRINT '? Tabla Products ya existe';
END
GO

PRINT '';
PRINT '========================================';
PRINT '  Base de datos inicializada';
PRINT '========================================';
"@

# Guardar el script SQL en un archivo
$sqlFilePath = "CreateDatabaseTables.sql"
$sqlScript | Out-File -FilePath $sqlFilePath -Encoding UTF8
Write-Host "? Script SQL generado: $sqlFilePath" -ForegroundColor Green
Write-Host ""

Write-Host "OPCIÓN 3: Ejecutar el script SQL automáticamente" -ForegroundColor Green
Write-Host "----------------------------------------" -ForegroundColor Green
$response = Read-Host "¿Deseas ejecutar el script SQL ahora? (S/N)"

if ($response -eq 'S' -or $response -eq 's') {
    try {
        $serverName = "DESKTOP-40FEK5D\MSSQLSERVERJC"
        $databaseName = "CRM"
        $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True;"
        
        Write-Host ""
        Write-Host "Ejecutando script SQL..." -ForegroundColor Yellow
        
        $connection = New-Object System.Data.SqlClient.SqlConnection
        $connection.ConnectionString = $connectionString
        $connection.Open()
        
        # Dividir y ejecutar cada batch
        $batches = $sqlScript -split 'GO'
        
        foreach ($batch in $batches) {
            if ($batch.Trim() -ne '') {
                $command = $connection.CreateCommand()
                $command.CommandText = $batch
                $command.ExecuteNonQuery() | Out-Null
            }
        }
        
        $connection.Close()
        
        Write-Host ""
        Write-Host "? Script ejecutado exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Verificando tablas creadas..." -ForegroundColor Yellow
        
        # Ejecutar script de verificación
        .\TestDatabaseConnection.ps1
    }
    catch {
        Write-Host ""
        Write-Host "? Error al ejecutar el script:" -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
    }
}
else {
    Write-Host ""
    Write-Host "Puedes ejecutar el script manualmente:" -ForegroundColor Yellow
    Write-Host "  sqlcmd -S DESKTOP-40FEK5D\MSSQLSERVERJC -d CRM -i $sqlFilePath" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "O ábrelo en SQL Server Management Studio" -ForegroundColor Yellow
}
