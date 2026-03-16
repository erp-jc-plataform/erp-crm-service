#!/usr/bin/env pwsh
# push-all-repos.ps1
# Commit y push de todos los microservicios a sus repos en erp-jc-plataform

$ErrorActionPreference = "Continue"

$repos = @(
    @{ path = "Business-Gateway";        remote = "https://github.com/erp-jc-plataform/erp-gateway-service.git";        branch = "main" },
    @{ path = "Business-Security";       remote = "https://github.com/erp-jc-plataform/erp-security-service.git";       branch = "main" },
    @{ path = "Business-Licensing";      remote = "https://github.com/erp-jc-plataform/erp-licencias-service.git";      branch = "main" },
    @{ path = "Business-Mensajeria";     remote = "https://github.com/erp-jc-plataform/erp-mensajeria-service.git";     branch = "main" },
    @{ path = "Business-Notificaciones"; remote = "https://github.com/erp-jc-plataform/erp-notifications-service.git"; branch = "main" },
    @{ path = "Business-Report";         remote = "https://github.com/erp-jc-plataform/erp-report-service.git";         branch = "main" },
    @{ path = "Business-Log";            remote = "https://github.com/erp-jc-plataform/erp-log-service.git";            branch = "main" },
    @{ path = "Business-Mobile";         remote = "https://github.com/erp-jc-plataform/erp-mobile-service.git";         branch = "main" },
    @{ path = "Business-CRM";            remote = "https://github.com/erp-jc-plataform/erp-crm-service.git";            branch = "main" }
)

$commitMessage = if ($args[0]) { $args[0] } else { "feat: multi-tenant GoRouter + dashboard KPIs" }
$root = $PSScriptRoot

foreach ($repo in $repos) {
    $fullPath = Join-Path $root $repo.path
    if (-Not (Test-Path $fullPath)) {
        Write-Host "SKIP $($repo.path) - carpeta no encontrada" -ForegroundColor Yellow
        continue
    }

    Write-Host "" 
    Write-Host "-> $($repo.path)" -ForegroundColor Cyan
    Push-Location $fullPath

    $remoteUrl = git remote get-url origin 2>$null
    if ($LASTEXITCODE -ne 0) {
        git remote add origin $repo.remote
    } elseif ($remoteUrl.Trim() -ne $repo.remote) {
        git remote set-url origin $repo.remote
    }

    if (-Not (Test-Path ".git")) {
        git init
        git checkout -b $repo.branch 2>$null
    }

    git add -A
    $status = git status --porcelain
    if ($status) {
        git commit -m $commitMessage
        Write-Host "   Commit OK" -ForegroundColor Green
    } else {
        Write-Host "   Sin cambios" -ForegroundColor Gray
    }

    git push -u origin $repo.branch
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   Push OK" -ForegroundColor Green
    } else {
        Write-Host "   Push FAIL (revisar credenciales)" -ForegroundColor Red
    }

    Pop-Location
}

Write-Host ""
Write-Host "Todos los repos procesados." -ForegroundColor Green
