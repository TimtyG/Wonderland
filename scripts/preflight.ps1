<#
.SYNOPSIS
Checks whether the Wonderland Private Server checkout has the common build/runtime prerequisites.

.DESCRIPTION
This script is intentionally read-only. It does not download packages, create databases, or supply game/client assets.
Run it from the repository root in Windows PowerShell.
#>

$ErrorActionPreference = 'Continue'
$repo = Split-Path -Parent $PSScriptRoot
Set-Location $repo

$failures = 0
$warnings = 0

function Pass($message) {
    Write-Host "[PASS] $message" -ForegroundColor Green
}

function Warn($message) {
    $script:warnings++
    Write-Host "[WARN] $message" -ForegroundColor Yellow
}

function Fail($message) {
    $script:failures++
    Write-Host "[FAIL] $message" -ForegroundColor Red
}

function Test-PathReadable($path, $required = $true) {
    if (Test-Path -LiteralPath $path) {
        Pass "Found $path"
    }
    elseif ($required) {
        Fail "Missing required file/folder: $path"
    }
    else {
        Warn "Missing optional file/folder: $path"
    }
}

Write-Host "Wonderland Private Server preflight" -ForegroundColor Cyan
Write-Host "Repository: $repo"
Write-Host ""

Test-PathReadable 'Wonderland Private Server.sln'
Test-PathReadable 'Wonderland Private Server.csproj'
Test-PathReadable 'wlo.pserver.core\wlo.pserver.core.csproj'
Test-PathReadable 'packages.config'
Test-PathReadable '.nuget\NuGet.exe'

Write-Host ""
Write-Host "Checking bundled DLLs..." -ForegroundColor Cyan
@(
    'DLLS\GupdtSrv.dll',
    'DLLS\Octokit.dll',
    'DLLS\Phoenix.Core.dll',
    'DLLS\PhoenixData.dll',
    'DLLS\RCLibrary.dll',
    'DLLS\System.Data.SQLite.dll',
    'DLLS\Wlo.Core.dll',
    'x86\SQLite.Interop.dll',
    'x64\SQLite.Interop.dll'
) | ForEach-Object { Test-PathReadable $_ }

Write-Host ""
Write-Host "Checking required runtime game assets..." -ForegroundColor Cyan
Test-PathReadable 'Data\itemDat.wpdat'
if (Test-Path -LiteralPath 'Maps') {
    $mapDlls = Get-ChildItem -LiteralPath 'Maps' -Filter '*.dll' -Recurse -ErrorAction SilentlyContinue
    if ($mapDlls.Count -gt 0) {
        Pass "Found $($mapDlls.Count) map plugin DLL(s) under Maps"
    }
    else {
        Fail 'Maps folder exists, but no map plugin DLLs were found'
    }
}
else {
    Fail 'Missing Maps folder with compiled map plugin DLLs'
}

Write-Host ""
Write-Host "Checking build tools..." -ForegroundColor Cyan
$msbuild = Get-Command msbuild.exe -ErrorAction SilentlyContinue
if ($msbuild) {
    Pass "Found MSBuild: $($msbuild.Source)"
}
else {
    Warn 'MSBuild was not found on PATH. Build from a Visual Studio Developer Command Prompt, or open the solution in Visual Studio.'
}

$nuget = Get-Command nuget.exe -ErrorAction SilentlyContinue
if ($nuget) {
    Pass "Found NuGet on PATH: $($nuget.Source)"
}
else {
    Warn 'NuGet was not found on PATH. The repo includes .nuget\NuGet.exe, and Visual Studio may restore packages automatically.'
}

Write-Host ""
Write-Host "Checking user settings..." -ForegroundColor Cyan
$config = Join-Path $env:APPDATA 'PServer\Config.settings.wlo'
if (Test-Path -LiteralPath $config) {
    Pass "Found settings file: $config"
}
else {
    Warn "Settings file not found yet: $config"
}

Write-Host ""
if ($failures -gt 0) {
    Write-Host "Preflight finished with $failures failure(s) and $warnings warning(s)." -ForegroundColor Red
    Write-Host 'You must resolve failures before this server is likely to start.' -ForegroundColor Red
    exit 1
}
elseif ($warnings -gt 0) {
    Write-Host "Preflight finished with $warnings warning(s)." -ForegroundColor Yellow
    exit 0
}
else {
    Write-Host 'Preflight passed.' -ForegroundColor Green
    exit 0
}
