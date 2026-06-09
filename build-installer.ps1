# build-installer.ps1
# ============================================================
#  Pubblica DicomModifier e compila l'installer Inno Setup.
#
#  Utilizzo:
#    .\build-installer.ps1
#
#  Prerequisiti:
#    - .NET 8 SDK
#    - Inno Setup 6  (https://jrsoftware.org/isdl.php)
# ============================================================

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot   = $PSScriptRoot
$slnFile    = Join-Path $repoRoot "Dicom Modifier.sln"
$publishDir = Join-Path $repoRoot "publish"
$issFile    = Join-Path $repoRoot "Installer\DicomModifier.iss"
$iscc       = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

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

if (-not (Test-Path $iscc)) {
	Write-Error "Inno Setup non trovato in: $iscc`nScaricalo da https://jrsoftware.org/isdl.php"
	exit 1
}

& $iscc $issFile

if ($LASTEXITCODE -ne 0) {
	Write-Error "ISCC.exe fallito (exit code $LASTEXITCODE)."
	exit $LASTEXITCODE
}

$outputDir = Join-Path $repoRoot "Installer\output"
Write-Host "`n[2/2] Installer pronto in: $outputDir" -ForegroundColor Green
Get-ChildItem $outputDir -Filter "*.exe" | ForEach-Object {
	Write-Host "  -> $($_.Name)  ($([math]::Round($_.Length/1MB,1)) MB)"
}
