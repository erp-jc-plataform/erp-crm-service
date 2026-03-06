# Script simple para ejecutar el SQL de creación de tablas
param(
    [string]$SqlFile = "CreateDatabaseTables.sql"
)

$serverName = "DESKTOP-40FEK5D\MSSQLSERVERJC"
$databaseName = "CRM"

Write-Host "Ejecutando script SQL: $SqlFile" -ForegroundColor Cyan
Write-Host "Servidor: $serverName" -ForegroundColor Yellow
Write-Host "Base de datos: $databaseName" -ForegroundColor Yellow
Write-Host ""

try {
    # Leer el archivo SQL
    $sqlContent = Get-Content -Path $SqlFile -Raw
    
    # Crear conexión
    $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;TrustServerCertificate=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # Dividir por GO y ejecutar cada batch
    $batches = $sqlContent -split '\r?\nGO\r?\n'
    $batchNumber = 0
    
    foreach ($batch in $batches) {
        $trimmedBatch = $batch.Trim()
        if ($trimmedBatch -ne '' -and $trimmedBatch -notmatch '^\s*--') {
            $batchNumber++
            Write-Host "Ejecutando batch $batchNumber..." -ForegroundColor Gray
            
            $command = New-Object System.Data.SqlClient.SqlCommand($trimmedBatch, $connection)
            $command.ExecuteNonQuery() | Out-Null
        }
    }
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "Script ejecutado exitosamente!" -ForegroundColor Green
    Write-Host ""
    
    # Verificar tablas
    Write-Host "Verificando tablas creadas..." -ForegroundColor Cyan
    .\TestDatabaseConnection.ps1
}
catch {
    Write-Host ""
    Write-Host "Error al ejecutar el script:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
