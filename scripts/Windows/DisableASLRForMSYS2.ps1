[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][String]$MsysRoot
)

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'Stop'

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Use Administrator to run this script."
    return
}

Write-Host "Considering MSYS2 Root: ${MsysRoot}"
$files = (Get-ChildItem "${MsysRoot}\usr\bin\*.exe").Name
Write-Host "Total $($files.Count) executables will be affected."
pause

$files.ForEach({Set-ProcessMitigation -Verbose -Name $_ -Disable ForceRelocateImages,BottomUp})
