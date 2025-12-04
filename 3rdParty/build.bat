@echo off
setlocal enabledelayedexpansion
set ERRORLEVEL=0

REM ===================================================
REM Build all
REM ===================================================

call stb_image\build.bat
if errorlevel 1 goto :error

call tiny_obj_loader\build.bat
if errorlevel 1 goto :error

echo.
echo Build complete.
goto :eof

:error
echo.
echo Build failed!
exit /b 1