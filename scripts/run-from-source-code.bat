@echo off

:start

cd /d "%~dp0\.."

set /p name=Enter project name:

if /i "%name%"=="exit" goto exit

if not exist "source-codes\%name%" (
  echo Project %name% does not exist.
  echo.
  goto start
)

cd /d "source-codes\%name%"

dotnet run

goto start

:exit
exit