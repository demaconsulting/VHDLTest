#!/bin/bash

# Comprehensive Linting Script
#
# PURPOSE:
# - Run ALL lint checks when executed (no options or modes)
# - Output lint failures directly for agent parsing
# - NO command-line arguments, pretty printing, or colorization
# - Agents execute this script to identify files needing fixes

lint_error=0

# Install npm dependencies
npm install || exit 1

# Create Python virtual environment (for yamllint)
if [ ! -d ".venv" ]; then
  python -m venv .venv || exit 1
fi
source .venv/bin/activate || exit 1
pip install -r pip-requirements.txt || exit 1

# Run spell check
npx cspell --no-progress --no-color "**/*.{md,yaml,yml,json,cs,txt}" || lint_error=1

# Run markdownlint check
npx markdownlint-cli2 "**/*.md" || lint_error=1

# Run yamllint check
yamllint -c .yamllint.yaml . || lint_error=1

# Run .NET formatting check (verifies no changes are needed)
dotnet format --verify-no-changes DEMAConsulting.VHDLTest.sln || lint_error=1

exit $lint_error
