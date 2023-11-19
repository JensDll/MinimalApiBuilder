$env:MINIMALAPIBUILDER_TEST_TYPE = 'Local'
$env:DOTNET_CLI_USE_MSBUILD_SERVER = 'false'

$solution = Join-Path $PSScriptRoot .. MinimalApiBuilder.sln

Start-Process -FilePath "$env:VSINSTALLDIR\Common7\IDE\devenv.com" -ArgumentList $solution
