# Script de instalación automatizada de Swagger para ERP CRM API
# Este script instala los paquetes NuGet necesarios y configura el proyecto

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "   INSTALACIÓN DE SWAGGER EN ERP CRM API" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "src\ErpCrmService.WebApi\ErpCrmService.WebApi.csproj"

# Verificar que el proyecto existe
if (-not (Test-Path $projectPath)) {
    Write-Host "ERROR: No se encontró el proyecto WebApi" -ForegroundColor Red
    Write-Host "Ruta esperada: $projectPath" -ForegroundColor Yellow
    exit 1
}

Write-Host "Proyecto encontrado: $projectPath" -ForegroundColor Green
Write-Host ""

# Paso 1: Instalar paquetes NuGet
Write-Host "PASO 1: Instalando paquetes NuGet..." -ForegroundColor Yellow
Write-Host ""

$packages = @(
    @{Name="Swashbuckle"; Version="5.6.0"},
    @{Name="Microsoft.AspNet.WebApi"; Version="5.2.9"},
    @{Name="Microsoft.AspNet.WebApi.Cors"; Version="5.2.9"},
    @{Name="Newtonsoft.Json"; Version="13.0.3"}
)

foreach ($package in $packages) {
    Write-Host "  Instalando $($package.Name) v$($package.Version)..." -ForegroundColor Gray
    
    try {
        $command = "nuget install $($package.Name) -Version $($package.Version) -OutputDirectory packages -NonInteractive"
        Invoke-Expression $command | Out-Null
        Write-Host "  ? $($package.Name) instalado" -ForegroundColor Green
    }
    catch {
        Write-Host "  ? Error instalando $($package.Name)" -ForegroundColor Yellow
        Write-Host "    Puedes instalarlo manualmente desde Visual Studio" -ForegroundColor Gray
    }
}

Write-Host ""

# Paso 2: Verificar archivos creados
Write-Host "PASO 2: Verificando archivos creados..." -ForegroundColor Yellow
Write-Host ""

$requiredFiles = @(
    "src\ErpCrmService.WebApi\App_Start\SwaggerConfig.cs",
    "src\ErpCrmService.WebApi\App_Start\WebApiConfig.cs",
    "src\ErpCrmService.WebApi\Controllers\HomeController.cs",
    "src\ErpCrmService.WebApi\Controllers\CustomersController.cs",
    "src\ErpCrmService.WebApi\Controllers\ProductsController.cs",
    "src\ErpCrmService.WebApi\Global.asax",
    "src\ErpCrmService.WebApi\Global.asax.cs"
)

$allFilesExist = $true
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "  ? $file" -ForegroundColor Green
    }
    else {
        Write-Host "  ? $file (NO ENCONTRADO)" -ForegroundColor Red
        $allFilesExist = $false
    }
}

Write-Host ""

if (-not $allFilesExist) {
    Write-Host "? ADVERTENCIA: Algunos archivos no se encontraron" -ForegroundColor Yellow
    Write-Host "  Revisa la guía: SWAGGER_IMPLEMENTATION_GUIDE.md" -ForegroundColor Gray
}

# Paso 3: Verificar Web.config
Write-Host "PASO 3: Verificando Web.config..." -ForegroundColor Yellow
Write-Host ""

$webConfigPath = "src\ErpCrmService.WebApi\Web.config"
if (Test-Path $webConfigPath) {
    Write-Host "  ? Web.config encontrado" -ForegroundColor Green
    
    $webConfigContent = Get-Content $webConfigPath -Raw
    if ($webConfigContent -match "DefaultConnection") {
        Write-Host "  ? Connection string configurado" -ForegroundColor Green
    }
    else {
        Write-Host "  ? Connection string no encontrado" -ForegroundColor Yellow
        Write-Host "    Agrega el connection string manualmente" -ForegroundColor Gray
    }
}
else {
    Write-Host "  ? Web.config no encontrado" -ForegroundColor Yellow
    Write-Host "    Crea el archivo Web.config manualmente" -ForegroundColor Gray
    Write-Host "    Ver: SWAGGER_IMPLEMENTATION_GUIDE.md" -ForegroundColor Gray
}

Write-Host ""

# Paso 4: Compilar proyecto
Write-Host "PASO 4: Compilando proyecto..." -ForegroundColor Yellow
Write-Host ""

try {
    Write-Host "  Compilando ErpCrmService.WebApi..." -ForegroundColor Gray
    $buildOutput = msbuild $projectPath /t:Build /p:Configuration=Debug /nologo 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Compilación exitosa" -ForegroundColor Green
    }
    else {
        Write-Host "  ? Compilación con advertencias/errores" -ForegroundColor Yellow
        Write-Host "    Abre Visual Studio para ver los detalles" -ForegroundColor Gray
    }
}
catch {
    Write-Host "  ? No se pudo compilar automáticamente" -ForegroundColor Yellow
    Write-Host "    Compila manualmente desde Visual Studio" -ForegroundColor Gray
}

Write-Host ""

# Resumen
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "   RESUMEN DE INSTALACIÓN" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "ARCHIVOS CREADOS:" -ForegroundColor Green
Write-Host "  • SwaggerConfig.cs - Configuración de Swagger" -ForegroundColor White
Write-Host "  • WebApiConfig.cs - Configuración de Web API" -ForegroundColor White
Write-Host "  • CustomersController.cs - API de Clientes" -ForegroundColor White
Write-Host "  • ProductsController.cs - API de Productos" -ForegroundColor White
Write-Host "  • HomeController.cs - Página de inicio" -ForegroundColor White
Write-Host "  • Global.asax / Global.asax.cs - Inicio de aplicación" -ForegroundColor White
Write-Host ""

Write-Host "PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "  1. Abre el proyecto en Visual Studio" -ForegroundColor White
Write-Host "  2. Restaura paquetes NuGet (clic derecho en solución > Restore NuGet Packages)" -ForegroundColor White
Write-Host "  3. Habilita XML documentation:" -ForegroundColor White
Write-Host "     - Properties > Build > XML documentation file" -ForegroundColor Gray
Write-Host "     - Ruta: bin\ErpCrmService.WebApi.XML" -ForegroundColor Gray
Write-Host "  4. Agrega referencias a Infrastructure y Domain" -ForegroundColor White
Write-Host "  5. Compila la solución (Ctrl + Shift + B)" -ForegroundColor White
Write-Host "  6. Ejecuta el proyecto (F5)" -ForegroundColor White
Write-Host "  7. Abre Swagger en: https://localhost:44300/swagger" -ForegroundColor White
Write-Host ""

Write-Host "DOCUMENTACIÓN:" -ForegroundColor Yellow
Write-Host "  Ver: SWAGGER_IMPLEMENTATION_GUIDE.md" -ForegroundColor White
Write-Host ""

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Abrir guía de implementación
$openGuide = Read-Host "¿Deseas abrir la guía de implementación? (S/N)"
if ($openGuide -eq 'S' -or $openGuide -eq 's') {
    if (Test-Path "SWAGGER_IMPLEMENTATION_GUIDE.md") {
        Start-Process "SWAGGER_IMPLEMENTATION_GUIDE.md"
    }
}

Write-Host "Instalación completada!" -ForegroundColor Green
Write-Host ""
