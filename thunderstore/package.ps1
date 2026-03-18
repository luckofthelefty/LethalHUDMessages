# Packages the mod for Thunderstore upload.
# Run from the repo root: .\thunderstore\package.ps1
#
# Requires: icon.png (256x256) in thunderstore/
# Produces: thunderstore/LuckOfTheLefty-LethalHUDMessages-1.0.0.zip

$ErrorActionPreference = "Stop"

$version = "1.0.0"
$outDir = "$PSScriptRoot\build"
$zipName = "LuckOfTheLefty-LethalHUDMessages-$version.zip"

# Clean
if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir | Out-Null

# Build
Push-Location "$PSScriptRoot\.."
dotnet build -c Release
Pop-Location

# Gather files
$dllSource = "$PSScriptRoot\..\LethalHUDMessages\bin\Release\netstandard2.1\com.github.luckofthelefty.LethalHUDMessages.dll"

Copy-Item "$PSScriptRoot\manifest.json" $outDir
Copy-Item "$PSScriptRoot\CHANGELOG.md" $outDir
Copy-Item "$PSScriptRoot\..\README.md" $outDir

if (Test-Path "$PSScriptRoot\icon.png") {
    Copy-Item "$PSScriptRoot\icon.png" $outDir
} else {
    Write-Warning "icon.png not found in thunderstore/ - you must add a 256x256 PNG before uploading!"
}

# Create plugins subfolder with the DLL
$pluginsDir = "$outDir\plugins"
New-Item -ItemType Directory -Path $pluginsDir | Out-Null
Copy-Item $dllSource $pluginsDir

# Zip
$zipPath = "$PSScriptRoot\$zipName"
if (Test-Path $zipPath) { Remove-Item $zipPath }
Compress-Archive -Path "$outDir\*" -DestinationPath $zipPath

Write-Host "Package created: $zipPath" -ForegroundColor Green
Write-Host "Upload this zip to https://thunderstore.io" -ForegroundColor Cyan
