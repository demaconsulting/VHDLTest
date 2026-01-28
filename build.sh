#!/usr/bin/env bash
# Build and test VHDLTest

set -e  # Exit on error

echo "🔧 Building VHDLTest..."
dotnet build --configuration Release

echo "✅ Running tests..."
dotnet test --configuration Release --verbosity normal

echo "✨ Build and test completed successfully!"
