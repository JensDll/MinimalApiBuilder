param(
  [string]$Sln = 'MinimalApiBuilder.sln'
)

$env:MINIMALAPIBUILDER_TEST_TYPE = 'Local'
$env:DOTNET_CLI_USE_MSBUILD_SERVER = 'false'

Start-Process -FilePath "$env:VSINSTALLDIR\Common7\IDE\devenv.com" -ArgumentList $Sln
