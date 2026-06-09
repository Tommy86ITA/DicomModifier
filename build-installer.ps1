# build-installer.ps1
# ============================================================
#  Pubblica DicomModifier e compila l'installer Inno Setup.
#
#  Utilizzo:
#    .\build-installer.ps1
#    .\build-installer.ps1 -InstallMissingTools
#
#  Prerequisiti:
#    - .NET 8 SDK
#    - Inno Setup 6
# ============================================================

param(
    [switch]$InstallMissingTools
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot   = $PSScriptRoot
$slnFile    = Join-Path $repoRoot "Dicom Modifier.sln"
$publishDir = Join-Path $repoRoot "publish"
$issFile    = Join-Path $repoRoot "Installer\DicomModifier.iss"

function Find-Iscc {
    $knownPaths = @(
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe"
    )

    foreach ($path in $knownPaths) {
        if (Test-Path $path) {
            return $path
        }
    }

    $command = Get-Command "ISCC.exe" -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    return $null
}

function Install-InnoSetup {
    Write-Host "Inno Setup non trovato. Tentativo di installazione tramite winget..." -ForegroundColor Yellow

    $winget = Get-Command "winget" -ErrorAction SilentlyContinue
    if (-not $winget) {
        throw "winget non è disponibile. Installa Inno Setup manualmente da https://jrsoftware.org/isdl.php"
    }

    winget install `
        --id JRSoftware.InnoSetup `
        --exact `
        --accept-package-agreements `
        --accept-source-agreements

    if ($LASTEXITCODE -ne 0) {
        throw "Installazione di Inno Setup tramite winget fallita (exit code $LASTEXITCODE)."
    }
}

# ── 0. Verifica Inno Setup ──────────────────────────────────
Write-Host "`n[0/2] Verifica Inno Setup..." -ForegroundColor Cyan

$iscc = Find-Iscc

if (-not $iscc) {
    if ($InstallMissingTools) {
        Install-InnoSetup
        $iscc = Find-Iscc
    }
    else {
        Write-Error "Inno Setup non trovato. Esegui lo script con -InstallMissingTools oppure installalo manualmente da https://jrsoftware.org/isdl.php"
        exit 1
    }
}

if (-not $iscc) {
    Write-Error "Inno Setup risulta installato, ma ISCC.exe non è stato trovato."
    exit 1
}

Write-Host "ISCC trovato: $iscc" -ForegroundColor Green

# ── 1. Publish ──────────────────────────────────────────────
Write-Host "`n[1/2] Pubblicazione in corso..." -ForegroundColor Cyan

if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
}

dotnet publish $slnFile `
    --configuration Release `
    --runtime win-x64 `
    --no-self-contained `
    -p:PublishProfile=Release-win-x64 `
    -p:PublishReadyToRun=true

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish fallito (exit code $LASTEXITCODE)."
    exit $LASTEXITCODE
}

Write-Host "[1/2] Publish completato -> $publishDir" -ForegroundColor Green

# ── 2. Inno Setup ───────────────────────────────────────────
Write-Host "`n[2/2] Compilazione installer..." -ForegroundColor Cyan

& $iscc $issFile

if ($LASTEXITCODE -ne 0) {
    Write-Error "ISCC.exe fallito (exit code $LASTEXITCODE)."
    exit $LASTEXITCODE
}

$outputDir = Join-Path $repoRoot "Installer\output"

Write-Host "`n[2/2] Installer pronto in: $outputDir" -ForegroundColor Green

Get-ChildItem $outputDir -Filter "*.exe" | ForEach-Object {
    Write-Host "  -> $($_.Name)  ($([math]::Round($_.Length / 1MB, 1)) MB)"
}