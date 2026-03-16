# Script de prueba para el Gateway

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  PRUEBAS DEL BUSINESS GATEWAY" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$baseUrl = "http://localhost:4000"

# Test 1: Info del Gateway
Write-Host "1. Probando /info..." -ForegroundColor Yellow
try {
    $info = Invoke-RestMethod -Uri "$baseUrl/info" -Method Get
    Write-Host "   ✓ Gateway: $($info.name) v$($info.version)" -ForegroundColor Green
    Write-Host "   ✓ Rutas configuradas: $($info.routes.Count)" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Health Check
Write-Host "`n2. Probando /health..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method Get
    Write-Host "   ✓ Estado: $($health.status)" -ForegroundColor Green
    Write-Host "   ✓ Servicios:" -ForegroundColor Green
    foreach ($service in $health.services) {
        $color = if ($service.status -eq "online") { "Green" } else { "Red" }
        Write-Host "      - $($service.name): $($service.status)" -ForegroundColor $color
    }
} catch {
    Write-Host "   ✗ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Login a través del Gateway
Write-Host "`n3. Probando LOGIN a través del Gateway..." -ForegroundColor Yellow
try {
    $loginBody = @{
        usuario = "admin"
        password = "admin123"
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginBody -Headers $headers
    
    if ($response.access_token) {
        Write-Host "   ✓ Login exitoso!" -ForegroundColor Green
        Write-Host "   ✓ Usuario: $($response.usuario)" -ForegroundColor Green
        Write-Host "   ✓ Token: $($response.access_token.Substring(0,20))..." -ForegroundColor Green
        
        # Guardar token para siguientes pruebas
        $global:token = $response.access_token
        
        # Test 4: Verificar token
        Write-Host "`n4. Verificando token en /api/auth/me..." -ForegroundColor Yellow
        $authHeaders = @{
            "Authorization" = "Bearer $global:token"
            "Content-Type" = "application/json"
        }
        
        $meResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/me" -Method Get -Headers $authHeaders
        Write-Host "   ✓ Token válido!" -ForegroundColor Green
        Write-Host "   ✓ Usuario: $($meResponse.usuario)" -ForegroundColor Green
        Write-Host "   ✓ Perfil: $($meResponse.perfil)" -ForegroundColor Green
        
    } else {
        Write-Host "   ✗ No se recibió token" -ForegroundColor Red
    }
} catch {
    $errorDetail = $_.ErrorDetails.Message | ConvertFrom-Json -ErrorAction SilentlyContinue
    if ($errorDetail) {
        Write-Host "   ✗ Error: $($errorDetail.detail)" -ForegroundColor Red
    } else {
        Write-Host "   ✗ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  FIN DE PRUEBAS" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan
