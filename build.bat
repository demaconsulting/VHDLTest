@echo off
REM Build and test VHDLTest (Windows)

echo Building VHDLTest...
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running unit tests...
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Build and test completed successfully!
