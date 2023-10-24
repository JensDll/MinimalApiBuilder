Get-ChildItem -Path $PSScriptRoot\..\tests\*\__snapshots__\**\*.received.cs `
| ForEach-Object {
  $newName = $_.Name -replace '.received.cs$', '.verified.cs'
  $destination = Join-Path $_.Directory $newName
  Move-Item -Path $_.FullName -Destination $destination -Force
}
