[CmdletBinding()]
param (
  [Parameter(Mandatory)]
  [string]$Version,
  [switch]$Unzip,
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Release'
)

$solutionFile = Join-Path $PSScriptRoot '..' 'MinimalApiBuilder.src.slnf'
$outputPath = Join-Path $PSScriptRoot '..' 'artifacts'

dotnet pack $solutionFile --no-restore `
  --output $outputPath -property:Version=$Version `
  --configuration $Configuration

if (-not (Test-Path "$outputPath")) {
  New-Item -ItemType Directory -Path "$outputPath"
}

if (-not $Unzip) {
  return;
}

Get-ChildItem $outputPath -Directory | Remove-Item -Recurse -Force

foreach ($file in Get-ChildItem $outputPath) {
  if ($file.Name -like '*.nupkg') {
    $filePath = $file.FullName
    $destinationPath = [regex]::Replace($filePath, '\.nupkg$', '')
    Expand-Archive -Path "$filePath" -DestinationPath "$destinationPath"
  }
}
