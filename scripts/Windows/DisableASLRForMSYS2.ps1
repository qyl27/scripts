[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][String]$msysRoot
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Use Administrator to run this script."
    return
}

Write-Host "Considering MSYS2 Root: ${msysRoot}"
$files = (Get-ChildItem "${msysRoot}\usr\bin\*.exe").FullName
Write-Host "Total $($files.Count) executables will be affected."
pause

$files.ForEach({Set-ProcessMitigation -Verbose $_ -Disable ForceRelocateImages})
$files.ForEach({Set-ProcessMitigation -Verbose $_ -Disable BottomUp})
