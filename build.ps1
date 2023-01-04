[CmdletBinding()]
param (
  [Parameter(Position = 0, Mandatory)]
  [ValidateSet('pack', 'test-nuget')]
  [Alias('t')]
  [string[]]$Targets,
  [string]$Configuration = 'Release',
  [string]$Version
)

$solution_file = Join-Path $PSScriptRoot "MinimalApiBuilder.Build.slnf"
$output_path = Join-Path $PSScriptRoot "dist"

switch ($Targets) {
  pack {
    dotnet pack $solution_file --configuration $Configuration --output $output_path -p:Version=$Version

    if (-not (Test-Path "$output_path")) {
      New-Item -ItemType Directory -Path "$output_path"
    }

    Get-ChildItem $output_path -Directory | Remove-Item -Recurse -Force

    foreach ($file in Get-ChildItem $output_path) {
      if ($file.Name -like "*.nupkg") {
        $file_path = $file.FullName
        $destination_path = [regex]::Replace($file_path, "\.nupkg$", "")
        Expand-Archive -Path "$file_path" -DestinationPath "$destination_path"
      }
    }
  }
  'test-nuget' {
    $configFile = Join-Path $PSScriptRoot "nuget.integration-test.config"

    dotnet restore .\test\MinimalApiBuilder.Generator.NugetIntegrationTest --packages ./packages --configfile $configFile

    dotnet build .\test\MinimalApiBuilder.Generator.NugetIntegrationTest `
      -c Release --packages ./packages --no-restore
  }
}
