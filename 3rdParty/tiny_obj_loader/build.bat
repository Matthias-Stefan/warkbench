@echo off
setlocal enabledelayedexpansion
set ERRORLEVEL=0

REM ===================================================
REM Project Paths
REM ===================================================
set SCRIPT_PATH=%~dp0
set SCRIPT_PATH=%SCRIPT_PATH:~0,-1%
set BIN_DIR=%SCRIPT_PATH%\bin
for %%I in ("%SCRIPT_PATH%\..") do set PARENT_PATH=%%~fI
set DLL_FILE=%PARENT_PATH%\tiny_obj_loader.dll

REM ===================================================
REM Clean Step
REM ===================================================
echo.
echo ---------------------------------------------------
echo Cleaning previous build artifacts
echo ---------------------------------------------------

if exist "%BIN_DIR%" (
    echo Cleaning bin directory: %BIN_DIR%
    rmdir /s /q "%BIN_DIR%"
)

if exist "%DLL_FILE%" (
    echo Removing previous DLL: %DLL_FILE%
    del /f /q "%DLL_FILE%"
)

REM ===================================================
REM Build tiny_obj_loader (DLL)
REM ===================================================
echo.
echo ---------------------------------------------------
echo Building tiny_obj_loader (.dll)
echo ---------------------------------------------------

echo Creating bin directory at "%BIN_DIR%"
mkdir "%BIN_DIR%"

REM Compiler and Flags
set CXX=clang++
set CXXFLAGS=-g -O0 -std=c++17 -Wall -Wextra -I. -I..\..
set SRC_FILE=%SCRIPT_PATH%\tiny_obj_loader_lib_export.cpp
set OBJ_FILE=%BIN_DIR%\tiny_obj_loader_lib_export.o

REM Compile
echo Compiling "%SRC_FILE%"...
%CXX% %CXXFLAGS% -c "%SRC_FILE%" -o "%OBJ_FILE%"
if errorlevel 1 goto :error

REM Link
echo Linking "%DLL_FILE%"...
%CXX% -shared -o "%DLL_FILE%" "%OBJ_FILE%"
if errorlevel 1 goto :error

echo.
echo Build complete.
goto :eof

:error
echo.
echo Build failed!
exit /b 1
