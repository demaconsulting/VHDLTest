#!/usr/bin/env bash
# Build and test VHDLTest

set -e  # Exit on error

echo "🔧 Building VHDLTest..."
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln

echo "🧪 Running unit tests..."
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln

echo "✨ Build and test completed successfully!"
