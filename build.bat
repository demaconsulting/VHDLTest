@echo off
REM Build and test VHDLTest (Windows)

echo Building VHDLTest...
dotnet build --configuration Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running tests...
dotnet test --configuration Release --verbosity normal
if %errorlevel% neq 0 exit /b %errorlevel%

echo Build and test completed successfully!
