Get-ChildItem -Path $PSScriptRoot\..\tests\*\__snapshots__ -Recurse -Filter *.received.cs `
| Select-Object @{ Name = "Path"; Expression = { $_.FullName } },
@{ Name = "Destination"; Expression = { Join-Path $_.Directory ($_.Name -replace '.received.cs$', '.verified.cs') } } `
| Move-Item -Force
