param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root "Estacionamento.sln"
$wapProj = Join-Path $root "Estacionamento.Package\Estacionamento.Package.wapproj"

Write-Host "Gerando icones..." -ForegroundColor Cyan
dotnet run --project (Join-Path $root "tools\GenerateAppIcons\GenerateAppIcons.csproj") -- (Join-Path $root "Estacionamento.Package\Images")

Write-Host "Compilando solucao ($Configuration|x64)..." -ForegroundColor Cyan
dotnet build $solution -c $Configuration

Write-Host "Empacotando MSIX..." -ForegroundColor Cyan
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" `
    | Select-Object -First 1

if (-not $msbuild) {
    Write-Error "MSBuild nao encontrado. Instale o Visual Studio com 'Ferramentas de empacotamento MSIX'."
}

& $msbuild $wapProj `
    /p:Configuration=$Configuration `
    /p:Platform=x64 `
    /p:AppxPackageSigningEnabled=false `
    /p:UapAppxPackageBuildMode=StoreUpload `
    /p:GenerateAppxPackageOnBuild=true

Write-Host "Concluido. Verifique a pasta Estacionamento.Package\AppPackages\" -ForegroundColor Green
