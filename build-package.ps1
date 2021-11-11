#Requires -Version 5.1

[CmdletBinding()]
param (
  [string] $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

if (Test-Path -Path output) {
  Write-Host 'Cleaning up existing build output...'
  Remove-Item -Recurse output
}

Write-Host "Running package restore..."
dotnet restore

Write-Host "Building the solution ($Configuration mode)..."
dotnet build --nologo --no-restore -c:$Configuration '-p:ContinuousIntegrationBuild=true' '-p:Deterministic=true'

Write-Host "Running tests..."
dotnet test --nologo --no-build -c:$Configuration

Write-Host "Creating packages..."
dotnet pack --nologo --no-build -c:$Configuration
