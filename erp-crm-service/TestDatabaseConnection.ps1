# Script para verificar la conexión a la base de datos CRM
# y el estado de las tablas

$serverName = "DESKTOP-40FEK5D\MSSQLSERVERJC"
$databaseName = "CRM"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test de Conexión a Base de Datos CRM" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

try {
    # Crear cadena de conexión
    $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True;"
    
    # Crear conexión
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    
    # Abrir conexión
    Write-Host "Conectando a: $serverName" -ForegroundColor Yellow
    Write-Host "Base de datos: $databaseName" -ForegroundColor Yellow
    Write-Host ""
    
    $connection.Open()
    Write-Host "? Conexión exitosa!" -ForegroundColor Green
    Write-Host ""
    
    # Verificar tablas existentes
    $query = @"
SELECT 
    TABLE_NAME,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME
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
    
    if ($tables.Count -eq 0) {
        Write-Host "? No se encontraron tablas en la base de datos." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "SIGUIENTE PASO:" -ForegroundColor Cyan
        Write-Host "Ejecuta el siguiente comando en la Package Manager Console de Visual Studio:" -ForegroundColor White
        Write-Host ""
        Write-Host "  Update-Database" -ForegroundColor Green
        Write-Host ""
        Write-Host "O si no tienes migraciones habilitadas:" -ForegroundColor White
        Write-Host ""
        Write-Host "  Enable-Migrations" -ForegroundColor Green
        Write-Host "  Add-Migration InitialCreate" -ForegroundColor Green
        Write-Host "  Update-Database" -ForegroundColor Green
    }
    else {
        Write-Host "Tablas encontradas en la base de datos:" -ForegroundColor Green
        Write-Host ""
        $tables | Format-Table -AutoSize
        
        # Verificar si existen las tablas esperadas
        $expectedTables = @("Customers", "Products")
        $missingTables = @()
        
        foreach ($expectedTable in $expectedTables) {
            if ($tables.TableName -notcontains $expectedTable) {
                $missingTables += $expectedTable
            }
        }
        
        if ($missingTables.Count -gt 0) {
            Write-Host ""
            Write-Host "? Tablas esperadas faltantes:" -ForegroundColor Yellow
            $missingTables | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
            Write-Host ""
            Write-Host "Ejecuta Update-Database para crear las tablas." -ForegroundColor Cyan
        }
        else {
            Write-Host ""
            Write-Host "? Todas las tablas necesarias están presentes." -ForegroundColor Green
            
            # Contar registros en cada tabla
            Write-Host ""
            Write-Host "Conteo de registros:" -ForegroundColor Cyan
            foreach ($table in $expectedTables) {
                $countQuery = "SELECT COUNT(*) FROM [$table]"
                $countCommand = $connection.CreateCommand()
                $countCommand.CommandText = $countQuery
                $count = $countCommand.ExecuteScalar()
                Write-Host "  $table : $count registros" -ForegroundColor White
            }
        }
    }
    
    $connection.Close()
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Prueba completada exitosamente" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "? Error al conectar a la base de datos:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifica que:" -ForegroundColor Yellow
    Write-Host "  1. El servicio SQL Server esté corriendo" -ForegroundColor White
    Write-Host "  2. El nombre del servidor sea correcto: $serverName" -ForegroundColor White
    Write-Host "  3. La base de datos '$databaseName' exista" -ForegroundColor White
    Write-Host "  4. Tengas permisos de acceso" -ForegroundColor White
}
finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
