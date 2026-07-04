# Demo GraphQL Services Launcher
# This script starts both the Users and Products GraphQL services

Write-Host "Starting Demo GraphQL Services..." -ForegroundColor Green
Write-Host ""

$rootPath = Get-Location

# Start Users Service
Write-Host "Starting Users Service on http://localhost:4001/graphql..." -ForegroundColor Cyan
$usersJob = Start-Job -ScriptBlock {
	param($root)
	Set-Location "$root\src\RoverDotNet.Demo.Api.Users"
	dotnet run --no-build
} -ArgumentList $rootPath

# Wait a moment before starting the second service
Start-Sleep -Seconds 2

# Start Products Service
Write-Host "Starting Products Service on http://localhost:4002/graphql..." -ForegroundColor Cyan
$productsJob = Start-Job -ScriptBlock {
	param($root)
	Set-Location "$root\src\RoverDotNet.Demo.Api.Products"
	dotnet run --no-build
} -ArgumentList $rootPath

Write-Host ""
Write-Host "Both services are starting up..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Access the services at:" -ForegroundColor Green
Write-Host "  Users Service:    http://localhost:4001/graphql" -ForegroundColor White
Write-Host "  Products Service: http://localhost:4002/graphql" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop both services" -ForegroundColor Yellow
Write-Host ""

# Monitor the jobs and display output
try {
	while ($true) {
		# Check if jobs are still running
		if ($usersJob.State -eq 'Failed' -or $productsJob.State -eq 'Failed') {
			Write-Host "One or more services failed to start. Check the output above." -ForegroundColor Red
			break
		}

		# Receive and display output from both jobs
		Receive-Job -Job $usersJob -ErrorAction SilentlyContinue
		Receive-Job -Job $productsJob -ErrorAction SilentlyContinue

		Start-Sleep -Milliseconds 500
	}
}
finally {
	# Clean up jobs when script is interrupted
	Write-Host ""
	Write-Host "Stopping services..." -ForegroundColor Yellow
	Stop-Job -Job $usersJob -ErrorAction SilentlyContinue
	Stop-Job -Job $productsJob -ErrorAction SilentlyContinue
	Remove-Job -Job $usersJob -Force -ErrorAction SilentlyContinue
	Remove-Job -Job $productsJob -Force -ErrorAction SilentlyContinue
	Write-Host "Services stopped." -ForegroundColor Green
}
