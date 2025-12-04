#!/bin/bash
set -e

# ===================================================
# Project Paths
# ===================================================
SCRIPT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BIN_DIR="${SCRIPT_PATH}/bin"
PARENT_PATH="$(dirname "$SCRIPT_PATH")"
SO_FILE="${PARENT_PATH}/tiny_obj_loader.so"

# ===================================================
# Clean Step
# ===================================================
echo
echo "---------------------------------------------------"
echo "Cleaning previous build artifacts"
echo "---------------------------------------------------"

if [ -d "$BIN_DIR" ]; then
    echo "Cleaning bin directory: $BIN_DIR"
    rm -rf "$BIN_DIR"
fi

if [ -f "$SO_FILE" ]; then
    echo "Removing previous shared library: $SO_FILE"
    rm "$SO_FILE"
fi

# ===================================================
# Build tiny_obj_loader (Shared Library)
# ===================================================
echo
echo "---------------------------------------------------"
echo "Building tiny_obj_loader (.so)"
echo "---------------------------------------------------"

# Create bin directory
echo "Creating bin directory at \"$BIN_DIR\""
mkdir -p "$BIN_DIR"

# Compiler and Flags
CXX=clang++
CXXFLAGS="-g -O0 -fPIC -std=c++17"
INCLUDES="-I. -I../.."

# Source
SRC_FILE="${SCRIPT_PATH}/tiny_obj_loader_lib_export.cpp"
OBJ_FILE="${BIN_DIR}/tiny_obj_loader_lib_export.o"

# Compile
echo "Compiling \"$SRC_FILE\"..."
$CXX $CXXFLAGS $INCLUDES -c "$SRC_FILE" -o "$OBJ_FILE"

# Link
echo "Linking \"$SO_FILE\"..."
$CXX -shared -o "$SO_FILE" "$OBJ_FILE"

echo
echo "Build complete."
