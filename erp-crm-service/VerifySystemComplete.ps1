# Script de Verificación Completa del Sistema CRM
# Verifica conectividad, estructura y datos

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  VERIFICACIÓN COMPLETA DEL SISTEMA CRM                      ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$serverName = "DESKTOP-40FEK5D\MSSQLSERVERJC"
$databaseName = "CRM"
$connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True;"

$results = @{
    Connection = $false
    DatabaseExists = $false
    TablesExist = $false
    IndexesExist = $false
    DataExists = $false
}

try {
    # 1. Verificar Conexión al Servidor
    Write-Host "1. ?? Verificando conexión al servidor..." -ForegroundColor Yellow
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Server=$serverName;Database=master;Integrated Security=True;TrustServerCertificate=True;"
    $connection.Open()
    Write-Host "   ? Conectado a: $serverName" -ForegroundColor Green
    $connection.Close()
    $results.Connection = $true
    
    # 2. Verificar Base de Datos
    Write-Host "2. ???  Verificando base de datos '$databaseName'..." -ForegroundColor Yellow
    $connection.ConnectionString = $connectionString
    $connection.Open()
    Write-Host "   ? Base de datos '$databaseName' accesible" -ForegroundColor Green
    $results.DatabaseExists = $true
    
    # 3. Verificar Tablas
    Write-Host "3. ?? Verificando tablas..." -ForegroundColor Yellow
    $query = @"
SELECT 
    t.TABLE_NAME,
    COUNT(c.COLUMN_NAME) as ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME
"@
    
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $reader = $command.ExecuteReader()
    
    $tables = @()
    while ($reader.Read()) {
        $tables += [PSCustomObject]@{
            TableName = $reader["TABLE_NAME"]
            ColumnCount = $reader["ColumnCount"]
        }
    }
    $reader.Close()
    
    $expectedTables = @{
        "Customers" = 22
        "Products" = 23
    }
    
    $allTablesExist = $true
    foreach ($tableName in $expectedTables.Keys) {
        $table = $tables | Where-Object { $_.TableName -eq $tableName }
        if ($table) {
            $expectedColumns = $expectedTables[$tableName]
            if ($table.ColumnCount -eq $expectedColumns) {
                Write-Host "   ? Tabla '$tableName' existe con $($table.ColumnCount) columnas" -ForegroundColor Green
            }
            else {
                Write-Host "   ? Tabla '$tableName' tiene $($table.ColumnCount) columnas (esperadas: $expectedColumns)" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "   ? Tabla '$tableName' NO existe" -ForegroundColor Red
            $allTablesExist = $false
        }
    }
    
    if ($allTablesExist) {
        $results.TablesExist = $true
    }
    
    # 4. Verificar Índices
    Write-Host "4. ?? Verificando índices..." -ForegroundColor Yellow
    $indexQuery = @"
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.is_unique AS IsUnique
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
    AND t.name IN ('Customers', 'Products')
ORDER BY t.name, i.name
"@
    
    $command.CommandText = $indexQuery
    $reader = $command.ExecuteReader()
    
    $indexes = @()
    while ($reader.Read()) {
        $indexes += [PSCustomObject]@{
            TableName = $reader["TableName"]
            IndexName = $reader["IndexName"]
            IsUnique = $reader["IsUnique"]
        }
    }
    $reader.Close()
    
    $expectedIndexes = @(
        "IX_Customer_Email",
        "IX_Customer_TaxId",
        "IX_Customer_CompanyName",
        "IX_Customer_Status_Active",
        "IX_Product_SKU",
        "IX_Product_Name",
        "IX_Product_Category_Active",
        "IX_Product_Stock"
    )
    
    $allIndexesExist = $true
    foreach ($indexName in $expectedIndexes) {
        $index = $indexes | Where-Object { $_.IndexName -eq $indexName }
        if ($index) {
            $uniqueText = if ($index.IsUnique) { "(Único)" } else { "" }
            Write-Host "   ? Índice '$indexName' existe $uniqueText" -ForegroundColor Green
        }
        else {
            Write-Host "   ? Índice '$indexName' NO existe" -ForegroundColor Yellow
            $allIndexesExist = $false
        }
    }
    
    if ($allIndexesExist) {
        $results.IndexesExist = $true
    }
    
    # 5. Verificar Datos
    Write-Host "5. ?? Verificando datos..." -ForegroundColor Yellow
    
    $dataQuery = @"
SELECT 
    (SELECT COUNT(*) FROM Customers) as CustomerCount,
    (SELECT COUNT(*) FROM Products) as ProductCount
"@
    
    $command.CommandText = $dataQuery
    $reader = $command.ExecuteReader()
    
    if ($reader.Read()) {
        $customerCount = $reader["CustomerCount"]
        $productCount = $reader["ProductCount"]
        
        Write-Host "   ? Clientes: $customerCount registros" -ForegroundColor Green
        Write-Host "   ? Productos: $productCount registros" -ForegroundColor Green
        
        if ($customerCount -gt 0 -and $productCount -gt 0) {
            $results.DataExists = $true
        }
    }
    $reader.Close()
    
    # 6. Verificar productos con stock bajo
    Write-Host "6. ??  Verificando alertas de inventario..." -ForegroundColor Yellow
    
    $stockQuery = @"
SELECT Name, SKU, StockQuantity, MinimumStock
FROM Products
WHERE StockQuantity < MinimumStock
"@
    
    $command.CommandText = $stockQuery
    $reader = $command.ExecuteReader()
    
    $lowStockProducts = @()
    while ($reader.Read()) {
        $lowStockProducts += [PSCustomObject]@{
            Name = $reader["Name"]
            SKU = $reader["SKU"]
            Stock = $reader["StockQuantity"]
            Minimum = $reader["MinimumStock"]
        }
    }
    $reader.Close()
    
    if ($lowStockProducts.Count -gt 0) {
        Write-Host "   ? ALERTA: $($lowStockProducts.Count) producto(s) con stock bajo:" -ForegroundColor Yellow
        foreach ($product in $lowStockProducts) {
            Write-Host "     - $($product.Name) ($($product.SKU)): Stock=$($product.Stock), Mínimo=$($product.Minimum)" -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "   ? Todos los productos tienen stock adecuado" -ForegroundColor Green
    }
    
    $connection.Close()
    
    # Resumen Final
    Write-Host ""
    Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?  RESUMEN DE VERIFICACIÓN                                   ?" -ForegroundColor Cyan
    Write-Host "??????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
    $allChecks = $true
    foreach ($check in $results.Keys) {
        $status = if ($results[$check]) { "? PASS" } else { "? FAIL" }
        $color = if ($results[$check]) { "Green" } else { "Red" }
        Write-Host "  $status - $check" -ForegroundColor $color
        
        if (-not $results[$check]) {
            $allChecks = $false
        }
    }
    
    Write-Host ""
    if ($allChecks) {
        Write-Host "?? SISTEMA COMPLETAMENTE VERIFICADO Y OPERATIVO ??" -ForegroundColor Green
        Write-Host ""
        Write-Host "El sistema está listo para:" -ForegroundColor Cyan
        Write-Host "  • Desarrollo de nuevas funcionalidades" -ForegroundColor White
        Write-Host "  • Pruebas de integración" -ForegroundColor White
        Write-Host "  • Compilación y despliegue" -ForegroundColor White
    }
    else {
        Write-Host "??  ADVERTENCIA: Algunas verificaciones fallaron" -ForegroundColor Yellow
        Write-Host "Revisa los errores anteriores y ejecuta los scripts de inicialización necesarios." -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "???????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""
    
}
catch {
    Write-Host ""
    Write-Host "? ERROR durante la verificación:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifica que:" -ForegroundColor Yellow
    Write-Host "  1. SQL Server esté corriendo" -ForegroundColor White
    Write-Host "  2. El servidor '$serverName' sea accesible" -ForegroundColor White
    Write-Host "  3. Tengas permisos de acceso" -ForegroundColor White
    Write-Host "  4. La base de datos '$databaseName' exista" -ForegroundColor White
    Write-Host ""
}
finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
