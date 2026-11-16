$ErrorActionPreference = "Stop"
$source = Join-Path $PSScriptRoot "Resources"
$inventoryProject = Join-Path $PSScriptRoot "..\RPGInventoryProject"
$destination = Join-Path $inventoryProject "Resources"

if (-not (Test-Path $inventoryProject)) {
    throw "RPGInventoryProject was not found next to InventoryDemo.UI."
}

New-Item -ItemType Directory -Force -Path $destination | Out-Null
Copy-Item -Path (Join-Path $source "*") -Destination $destination -Recurse -Force
Write-Host "Resources copied to: $destination"
