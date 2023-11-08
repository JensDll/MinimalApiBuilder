param(
  [string]$Sln = 'MinimalApiBuilder.sln'
)

& $PSScriptRoot\activate.ps1

Start-Process -FilePath "$env:VSINSTALLDIR\Common7\IDE\devenv.com" -ArgumentList $Sln
