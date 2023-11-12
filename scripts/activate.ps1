param(
  [switch]$Published
)

if ($Published) {
  $env:MINIMALAPIBUILDER_TEST_TYPE = 'Published'
  $env:MINIMALAPIBUILDER_VERSION = '1.0.0-ci-published'
} else {
  $env:MINIMALAPIBUILDER_TEST_TYPE = 'Local'
}
