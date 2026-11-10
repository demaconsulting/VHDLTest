@echo off
setlocal

REM Comprehensive Linting Script
REM
REM PURPOSE:
REM - Run ALL lint checks when executed (no options or modes)
REM - Output lint failures directly for agent parsing
REM - NO command-line arguments, pretty printing, or colorization
REM - Agents execute this script to identify files needing fixes

set "LINT_ERROR=0"

REM === PYTHON SECTION ===

REM Create python venv if necessary
if not exist ".venv\Scripts\activate.bat" python -m venv .venv
if errorlevel 1 goto abort_python

REM Activate python venv
call .venv\Scripts\activate.bat
if errorlevel 1 goto abort_python

REM Install python tools
pip install -r pip-requirements.txt --quiet --disable-pip-version-check
if errorlevel 1 goto abort_python

REM Run yamllint
yamllint .
if errorlevel 1 set "LINT_ERROR=1"

REM Section error handling
goto npm_section
:abort_python
set "LINT_ERROR=1"
:npm_section

REM === SPELLING SECTION ===

REM Restore dotnet tools (required by TextLint and the compliance tools)
set "SKIP_DOTNET_TOOLS="
dotnet tool restore > nul
if errorlevel 1 goto abort_spelling

REM Run TextLint using the files configured in .textlint.yaml
dotnet textlint
if errorlevel 1 set "LINT_ERROR=1"

goto npm_section_start
:abort_spelling
set "LINT_ERROR=1"
set "SKIP_DOTNET_TOOLS=1"
:npm_section_start

REM === NPM SECTION ===

REM Install npm dependencies
set "PUPPETEER_SKIP_DOWNLOAD=true"
call npm install --silent
if errorlevel 1 goto abort_npm

REM Run markdownlint-cli2
call npx markdownlint-cli2 "**/*.md"
if errorlevel 1 set "LINT_ERROR=1"

REM Section error handling
goto dotnet_linting_section
:abort_npm
set "LINT_ERROR=1"
:dotnet_linting_section

REM === DOTNET LINTING SECTION ===

if defined SKIP_DOTNET_TOOLS goto dotnet_format_section

REM Run reqstream lint
dotnet reqstream --lint --requirements requirements.yaml
if errorlevel 1 set "LINT_ERROR=1"

REM Run versionmark lint
dotnet versionmark --lint
if errorlevel 1 set "LINT_ERROR=1"

REM Run reviewmark lint
dotnet reviewmark --lint
if errorlevel 1 set "LINT_ERROR=1"

REM Section error handling
goto dotnet_format_section
:abort_dotnet_tools
set "LINT_ERROR=1"
:dotnet_format_section

REM === DOTNET FORMATTING SECTION ===

REM Restore dotnet packages
dotnet restore > nul
if errorlevel 1 goto abort_dotnet_format

REM Run dotnet format
dotnet format --verify-no-changes --no-restore
if errorlevel 1 set "LINT_ERROR=1"

REM Section error handling
goto end
:abort_dotnet_format
set "LINT_ERROR=1"
:end

REM Report result
exit /b %LINT_ERROR%
