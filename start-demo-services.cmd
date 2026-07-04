@echo off
echo Starting Demo GraphQL Services...
echo.
echo This will open two command windows:
echo   - Users Service on http://localhost:4001/graphql
echo   - Products Service on http://localhost:4002/graphql
echo.
echo Press any key to start the services...
pause > nul

start "Users Service (Port 4001)" cmd /k "cd src\RoverDotNet.Demo.Api.Users && dotnet run"
timeout /t 2 /nobreak > nul
start "Products Service (Port 4002)" cmd /k "cd src\RoverDotNet.Demo.Api.Products && dotnet run"

echo.
echo Services are starting in separate windows.
echo Close those windows to stop the services.
echo.
pause
