Get-ChildItem -Path $PSScriptRoot\..\tests\*\__snapshots__ -Recurse -Include *.received.cs, *.received.txt `
| Select-Object @{ Name = 'Path'; Expression = { $_.FullName } },
@{ Name = 'Destination'; Expression = { Join-Path $_.Directory ($_.Name -replace '.received.(cs|txt)$', '.verified.$1') } } `
| Move-Item -Force
