$configFile = Join-Path $PSScriptRoot "nuget.integration-test.config"

dotnet restore .\test\MinimalApiBuilder.Generator.NugetIntegrationTest --packages ./packages --configfile $configFile

dotnet build .\test\MinimalApiBuilder.Generator.NugetIntegrationTest `
  -c Release --packages ./packages --no-restore
