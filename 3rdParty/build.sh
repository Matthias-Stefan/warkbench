#!/bin/bash
set -e

# ===================================================
# Build all
# ===================================================

./stb_image/build.sh
./tiny_obj_loader/build.sh

echo
echo "Build complete."
