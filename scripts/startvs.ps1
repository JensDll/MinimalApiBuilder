param(
  [string]$Sln = 'MinimalApiBuilder.sln'
)

$env:MINIMALAPIBUILDER_TEST_TYPE = 'Local'

Start-Process -FilePath "$env:VSINSTALLDIR\Common7\IDE\devenv.com" -ArgumentList $Sln
