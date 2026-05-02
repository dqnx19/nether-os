@echo off

REM Startup info
echo.
echo NetherOS Installation Utility
echo.
echo Type `help` to display help.
echo.

:start
set /p cmd=NOS: 

if /i "%cmd%"=="website" goto website
if /i "%cmd%"=="install user calculator" goto install_user_calculator
if /i "%cmd%"=="install system calculator" goto install_system_calculator

if /i "%cmd%"=="install user clocks" goto install_user_clocks
if /i "%cmd%"=="install system clocks" goto install_system_clocks

if /i "%cmd%"=="install user explorer" goto install_user_explorer
if /i "%cmd%"=="install system explorer" goto install_system_explorer

if /i "%cmd%"=="install user music" goto install_user_music
if /i "%cmd%"=="install system music" goto install_system_music

if /i "%cmd%"=="install user photos" goto install_user_photos
if /i "%cmd%"=="install system photos" goto install_system_photos

if /i "%cmd%"=="install user server" goto install_user_server
if /i "%cmd%"=="install system server" goto install_system_server

if /i "%cmd%"=="install user system-monitor" goto install_user_system-monitor
if /i "%cmd%"=="install system system-monitor" goto install_system_system-monitor

if /i "%cmd%"=="install user terminal" goto install_user_terminal
if /i "%cmd%"=="install system terminal" goto install_system_terminal

if /i "%cmd%"=="install user ui" goto install_user_ui
if /i "%cmd%"=="install system ui" goto install_system_ui

if /i "%cmd%"=="help" goto help
if /i "%cmd%"=="exit" goto exit

echo unknown Command `%cmd%`
echo.
goto start

:website
start msedge.exe https://netheros.netlify.app/
echo.
goto start

:help
echo Available Commands:
echo.
echo website            Displays website for more help.
echo install            Installs specific app.
echo help               Displays help.
echo exit               Close the program.
echo.
echo use help `command` for more information.
echo.
goto start


:exit
exit