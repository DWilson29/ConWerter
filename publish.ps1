#!/usr/bin/env pwsh
# ConWerter Multi-Platform Publishing Script
# Publishes self-contained builds for Windows, macOS, and Linux on x64 and ARM64 architectures

param(
	[string]$Configuration = "Release",
	[string]$OutputDir = ".\publish"
)

$ErrorActionPreference = "Stop"

# Define target platforms (Runtime Identifiers)
$platforms = @(
	"win-x64",
	"win-arm64",
	"osx-x64",
	"osx-arm64",
	"linux-x64",
	"linux-arm64"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ConWerter Multi-Platform Publisher" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Directory: $OutputDir" -ForegroundColor Yellow
Write-Host ""

# Create output directory if it doesn't exist
if (-not (Test-Path $OutputDir)) {
	New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
	Write-Host "Created output directory: $OutputDir" -ForegroundColor Green
}

$successCount = 0
$failCount = 0
$results = @()

# Helper function to map platform to OS name
function Get-OSName {
	param([string]$platform)

	if ($platform.StartsWith("win-")) { return "windows" }
	elseif ($platform.StartsWith("osx-")) { return "macos" }
	elseif ($platform.StartsWith("linux-")) { return "linux" }
	else { return "unknown" }
}

# Helper function to get architecture from platform
function Get-Architecture {
	param([string]$platform)

	if ($platform.EndsWith("-x64")) { return "x64" }
	elseif ($platform.EndsWith("-arm64")) { return "arm64" }
	else { return "unknown" }
}

foreach ($platform in $platforms) {
	Write-Host "----------------------------------------" -ForegroundColor Gray
	Write-Host "Publishing for: $platform" -ForegroundColor White
	Write-Host "----------------------------------------" -ForegroundColor Gray

	$platformOutput = Join-Path $OutputDir $platform

	try {
		# Execute dotnet publish command
		$publishArgs = @(
			"publish",
			"ConWerter.csproj",
			"-c", $Configuration,
			"-r", $platform,
			"--self-contained", "true",
			"-o", $platformOutput,
			"/p:PublishSingleFile=false",
			"/p:PublishTrimmed=false"
		)

		& dotnet @publishArgs
		$publishExitCode = $LASTEXITCODE

		if ($publishExitCode -eq 0) {
			Write-Host "Successfully published for $platform" -ForegroundColor Green

			# Create archive
			$osName = Get-OSName -platform $platform
			$arch = Get-Architecture -platform $platform
			$archiveName = "ConWerter-$osName-$arch"

			Write-Host "  Creating archive..." -ForegroundColor Cyan

			if ($platform.StartsWith("win-")) {
				# Use ZIP for Windows
				$archivePath = Join-Path $OutputDir "$archiveName.zip"
				if (Test-Path $archivePath) {
					Remove-Item $archivePath -Force
				}
				Compress-Archive -Path "$platformOutput\*" -DestinationPath $archivePath -CompressionLevel Optimal
				Write-Host "  Created: $archiveName.zip" -ForegroundColor Green
			}
			else {
				# Use tar.gz for macOS and Linux
				$archivePath = Join-Path $OutputDir "$archiveName.tar.gz"
				if (Test-Path $archivePath) {
					Remove-Item $archivePath -Force
				}

				# Use tar command (cross-platform compatible)
				$currentLocation = Get-Location
				Set-Location $platformOutput
				# Use absolute path and exclude the archive itself to prevent inclusion issues
				$absoluteArchivePath = [System.IO.Path]::GetFullPath($archivePath)
				& tar -czf $absoluteArchivePath --exclude=$([System.IO.Path]::GetFileName($archivePath)) .
				$tarExitCode = $LASTEXITCODE
				Set-Location $currentLocation

				if ($tarExitCode -eq 0) {
					Write-Host "  Created: $archiveName.tar.gz" -ForegroundColor Green
				}
				else {
					Write-Host "  Failed to create tar.gz archive" -ForegroundColor Yellow
				}
			}

			$successCount++
			$results += [PSCustomObject]@{
				Platform = $platform
				Status = "Success"
				OutputPath = $platformOutput
				ArchivePath = $archivePath
			}
		} else {
			throw "dotnet publish returned exit code $publishExitCode"
		}
	}
	catch {
		Write-Host "Failed to publish for $platform" -ForegroundColor Red
		Write-Host "  Error: $_" -ForegroundColor Red
		$failCount++
		$results += [PSCustomObject]@{
			Platform = $platform
			Status = "Failed"
			OutputPath = $null
			ArchivePath = $null
		}
	}

	Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$results | Format-Table -AutoSize

Write-Host ""
Write-Host "Total Platforms: $($platforms.Count)" -ForegroundColor White
Write-Host "Successful: $successCount" -ForegroundColor Green
Write-Host "Failed: $failCount" -ForegroundColor Red
Write-Host ""

if ($successCount -gt 0) {
	Write-Host "Published files are located in: $OutputDir" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "Archives created:" -ForegroundColor Cyan
	$successResults = $results | Where-Object { $_.Status -eq "Success" }
	foreach ($result in $successResults) {
		if ($result.ArchivePath) {
			$archiveFile = Split-Path $result.ArchivePath -Leaf
			$size = (Get-Item $result.ArchivePath).Length / 1MB
			$sizeRounded = [math]::Round($size, 2)
			Write-Host "  - $archiveFile ($sizeRounded MB)" -ForegroundColor White
		}
	}
}

if ($failCount -gt 0) {
	Write-Host "Warning: Some platforms failed to publish. Check errors above." -ForegroundColor Red
	exit 1
} else {
	Write-Host "All platforms published successfully!" -ForegroundColor Green
	exit 0
}
