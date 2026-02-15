# VHDLTest Template Consistency Analysis

**Date**: February 15, 2026  
**Template Repository**: [TemplateDotNetTool](https://github.com/demaconsulting/TemplateDotNetTool)  
**Template Version Reviewed**: Commit 25407e9 (most recent)  
**VHDLTest Repository**: Current working branch

## Executive Summary

This analysis identifies updates from the TemplateDotNetTool template that should be pulled into VHDLTest to maintain consistency with best practices. The most significant update is the **tool version tracking** feature (commit 25407e9), which adds automatic capture and reporting of tool versions to the Build Notes PDF.

### Priority Updates Identified

1. **HIGH PRIORITY**: Tool version tracking system (VersionMark integration)
2. **MEDIUM PRIORITY**: Build script enhancements (unit tests step)
3. **LOW PRIORITY**: Documentation clarifications and minor config updates

---

## 1. Tool Version Tracking (HIGHEST PRIORITY)

### Overview

The template added comprehensive tool version tracking that captures and reports all tool versions used during builds to the Build Notes PDF. This provides critical traceability for regulatory and auditing purposes.

**Template Commit**: 25407e9 (February 15, 2026)  
**PR**: #29 - "Add tool version tracking to Build Notes PDF"

### Changes Required in VHDLTest

#### 1.1 Add VersionMark Tool

**File**: `.config/dotnet-tools.json`

Add the following tool entry:

```json
"demaconsulting.versionmark": {
  "version": "0.1.0",
  "commands": [
    "versionmark"
  ]
}
```

#### 1.2 Create VersionMark Configuration

**File**: `.versionmark.yaml` (NEW FILE)

Create this file at the repository root with the following content (adapted for VHDLTest):

```yaml
---
# VersionMark Configuration File
# This file defines which tools to capture and how to extract their version information.

tools:
  # .NET SDK
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+(?:\.\d+)?)'

  # Git
  git:
    command: git --version
    regex: '(?i)git version (?<version>\d+\.\d+\.\d+)'

  # Node.js
  node:
    command: node --version
    regex: '(?i)v(?<version>\d+\.\d+\.\d+)'

  # npm
  npm:
    command: npm --version
    regex: '(?<version>\d+\.\d+\.\d+)'

  # SonarScanner for .NET (from dotnet tool list)
  dotnet-sonarscanner:
    command: dotnet tool list
    regex: '(?i)dotnet-sonarscanner\s+(?<version>\d+\.\d+\.\d+)'

  # Pandoc (DemaConsulting.PandocTool from dotnet tool list)
  pandoc:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.pandoctool\s+(?<version>\d+\.\d+\.\d+)'

  # WeasyPrint (DemaConsulting.WeasyPrintTool from dotnet tool list)
  weasyprint:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.weasyprinttool\s+(?<version>\d+\.\d+\.\d+)'

  # SarifMark (DemaConsulting.SarifMark from dotnet tool list)
  sarifmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.sarifmark\s+(?<version>\d+\.\d+\.\d+)'

  # SonarMark (DemaConsulting.SonarMark from dotnet tool list)
  sonarmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.sonarmark\s+(?<version>\d+\.\d+\.\d+)'

  # ReqStream (DemaConsulting.ReqStream from dotnet tool list)
  reqstream:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.reqstream\s+(?<version>\d+\.\d+\.\d+)'

  # BuildMark (DemaConsulting.BuildMark from dotnet tool list)
  buildmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.buildmark\s+(?<version>\d+\.\d+\.\d+)'

  # VersionMark (DemaConsulting.VersionMark from dotnet tool list)
  versionmark:
    command: dotnet tool list
    regex: '(?i)demaconsulting\.versionmark\s+(?<version>\d+\.\d+\.\d+)'

  # VHDLTest specific: GHDL simulator
  ghdl:
    command: ghdl --version
    regex: '(?i)GHDL (?<version>\d+\.\d+\.\d+)'

  # VHDLTest specific: NVC simulator  
  nvc:
    command: nvc --version
    regex: '(?i)nvc (?<version>\d+\.\d+(?:\.\d+)?)'
```

**Note**: The VHDLTest version includes additional tools specific to VHDL simulation (GHDL, NVC) that aren't in the template.

#### 1.3 Update .gitignore

**File**: `.gitignore`

Add these lines to ignore VersionMark capture files and generated documentation:

```gitignore
# Generated documentation (enhance existing section)
docs/requirements/requirements.md
docs/justifications/justifications.md
docs/tracematrix/tracematrix.md
docs/quality/codeql-quality.md
docs/quality/sonar-quality.md
docs/buildnotes.md
docs/buildnotes/versions.md

# VersionMark captures (generated during CI/CD)
versionmark-*.json

# Agent report files
AGENT_REPORT_*.md
```

**Current State**: VHDLTest's `.gitignore` has `AGENT_REPORT_*.md` but is missing the other entries.

#### 1.4 Update Build Notes Definition

**File**: `docs/buildnotes/definition.yaml`

Add the versions.md file to the input files list:

```yaml
input-files:
  - docs/buildnotes/title.txt
  - docs/buildnotes/introduction.md
  - docs/buildnotes.md
  - docs/buildnotes/versions.md  # ADD THIS LINE
```

#### 1.5 Update GitHub Workflow - Quality Checks Job

**File**: `.github/workflows/build.yaml`

Add version capture to the `quality-checks` job (after "Checkout" and "Setup dotnet" steps):

```yaml
      - name: Setup dotnet
        uses: actions/setup-dotnet@v5
        with:
          dotnet-version: 10.x

      - name: Restore Tools
        run: >
          dotnet tool restore

      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          dotnet versionmark --capture --job-id "quality" -- dotnet git versionmark
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-quality
          path: versionmark-quality.json
```

#### 1.6 Update GitHub Workflow - Build Job

**File**: `.github/workflows/build.yaml`

Add version capture to the `build` job (after the build/pack steps, before uploading test results):

```yaml
      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          # Create short job ID: build-win, build-ubuntu
          OS_SHORT=$(echo "${{ matrix.os }}" | sed 's/windows-latest/win/;s/ubuntu-latest/ubuntu/')
          JOB_ID="build-${OS_SHORT}"
          dotnet versionmark --capture --job-id "${JOB_ID}" -- \
            dotnet git dotnet-sonarscanner versionmark
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-${{ matrix.os }}
          path: versionmark-build-*.json
```

#### 1.7 Update GitHub Workflow - Integration Test Jobs

**File**: `.github/workflows/build.yaml`

For the integration test jobs (`test-ghdl` and `test-nvc`), add version capture steps. Note that these jobs need to checkout the repository first to access `.versionmark.yaml`:

Add after the initial steps but before installing the tool:

```yaml
      - name: Checkout
        uses: actions/checkout@v6
        with:
          sparse-checkout: |
            .versionmark.yaml
            .config/dotnet-tools.json

      # ... existing steps ...

      # Add before "Upload Test Results" step:
      - name: Capture tool versions
        shell: bash
        run: |
          echo "Capturing tool versions..."
          # Create appropriate job ID for the test job
          # For test-ghdl: int-ghdl-win-8, int-ghdl-ubuntu-8, etc.
          # For test-nvc: int-nvc-win-8, int-nvc-ubuntu-8, etc.
          OS_SHORT=$(echo "${{ matrix.os }}" | sed 's/windows-latest/win/;s/ubuntu-latest/ubuntu/')
          DOTNET_SHORT=$(echo "${{ matrix.dotnet }}" | sed 's/\.x$//')
          SIMULATOR="ghdl"  # or "nvc" for test-nvc job
          JOB_ID="int-${SIMULATOR}-${OS_SHORT}-${DOTNET_SHORT}"
          dotnet versionmark --capture --job-id "${JOB_ID}" -- \
            dotnet git versionmark ghdl  # or nvc
          echo "✓ Tool versions captured"

      - name: Upload version capture
        uses: actions/upload-artifact@v6
        with:
          name: version-capture-${{ matrix.os }}-dotnet${{ matrix.dotnet }}-ghdl  # or -nvc
          path: versionmark-int-*.json
```

#### 1.8 Update GitHub Workflow - Build Docs Job

**File**: `.github/workflows/build.yaml`

In the `build-docs` job, add these steps:

1. **After "Download CodeQL SARIF"**, add step to download all version captures:

```yaml
      - name: Download all version captures
        uses: actions/download-artifact@v7
        with:
          path: version-captures
          pattern: 'version-capture-*'
        continue-on-error: true
```

2. **After "Restore Tools"**, add version capture for build-docs:

```yaml
      - name: Capture tool versions for build-docs
        shell: bash
        run: |
          echo "Capturing tool versions..."
          dotnet versionmark --capture --job-id "build-docs" -- \
            dotnet git node npm pandoc weasyprint sarifmark sonarmark reqstream buildmark versionmark
          echo "✓ Tool versions captured"
```

3. **After "Generate build notes report with BuildMark"**, add version publishing:

```yaml
      - name: Publish Tool Versions
        shell: bash
        run: |
          echo "Publishing tool versions..."
          dotnet versionmark --publish --report docs/buildnotes/versions.md --report-depth 1 \
            -- "versionmark-*.json" "version-captures/**/versionmark-*.json"
          echo "✓ Tool versions published"

      - name: Display Tool Versions Report
        shell: bash
        run: |
          echo "=== Tool Versions Report ==="
          cat docs/buildnotes/versions.md
```

---

## 2. Build Script Enhancements

### Overview

The template updated build scripts to include a specific "unit tests" step with clearer messaging.

**Template Commit**: c6cb6aa (February 11, 2026)  
**PR**: #20 - "Add unit tests step to build scripts"

### Changes Required in VHDLTest

#### 2.1 Update build.sh

**File**: `build.sh`

Current VHDLTest build.sh:
```bash
#!/usr/bin/env bash
# Build and test VHDLTest

set -e  # Exit on error

echo "🔧 Building VHDLTest..."
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln

echo "✅ Running tests..."
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln

echo "✨ Build and test completed successfully!"
```

Template pattern:
```bash
#!/usr/bin/env bash
# Build and test VHDLTest

set -e  # Exit on error

echo "🔧 Building VHDLTest..."
dotnet build --configuration Release

echo "🧪 Running unit tests..."
dotnet test --configuration Release

echo "✅ Running self-validation..."
dotnet run --project src/DEMAConsulting.VHDLTest --configuration Release --framework net10.0 --no-build -- --validate

echo "✨ Build, tests, and validation completed successfully!"
```

**Analysis**: VHDLTest doesn't have the same self-validation pattern as the template (it has its own validation tests). The key improvement is the clearer test step naming.

**Recommended Change**:
```bash
#!/usr/bin/env bash
# Build and test VHDLTest

set -e  # Exit on error

echo "🔧 Building VHDLTest..."
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln

echo "🧪 Running unit tests..."
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln

echo "✨ Build and test completed successfully!"
```

#### 2.2 Update build.bat

**File**: `build.bat`

Current:
```batch
@echo off
REM Build and test VHDLTest (Windows)

echo Building VHDLTest...
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running tests...
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Build and test completed successfully!
```

**Recommended Change**:
```batch
@echo off
REM Build and test VHDLTest (Windows)

echo Building VHDLTest...
dotnet build --configuration Release DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Running unit tests...
dotnet test --configuration Release --verbosity normal DEMAConsulting.VHDLTest.sln
if %errorlevel% neq 0 exit /b %errorlevel%

echo Build and test completed successfully!
```

---

## 3. Documentation Updates

### 3.1 AGENTS.md Enhancements

Several clarifications were added to the template's AGENTS.md:

#### 3.1.1 Add CHANGELOG.md Documentation Note

**Template Commit**: 06eb9f2 (February 11, 2026)  
**PR**: #22 - "Document absence of CHANGELOG.md in AGENTS.md"

**File**: `AGENTS.md`

In the "Documentation" section (around line 70-80 in VHDLTest's version), add:

```markdown
## Documentation

- **User Guide**: `docs/guide/guide.md`
- **Requirements**: `requirements.yaml` -> auto-generated docs
- **Build Notes**: Auto-generated via BuildMark
- **Code Quality**: Auto-generated via CodeQL and SonarMark
- **Trace Matrix**: Auto-generated via ReqStream
- **CHANGELOG.md**: Not present - changes are captured in the auto-generated build notes
```

**Note**: VHDLTest doesn't currently have this "Documentation" section in AGENTS.md. Consider adding it.

#### 3.1.2 Update Markdownlint Reference

**Template Commit**: fcf9378 (February 12, 2026)  
**PR**: #23 - "Update documentation references from .markdownlint.json to .markdownlint-cli2.jsonc"

**File**: `AGENTS.md`, line 23

Change:
```markdown
- **`.cspell.json`, `.markdownlint.json`, `.yamllint.yaml`** - Linting configs
```

To:
```markdown
- **`.cspell.json`, `.markdownlint-cli2.jsonc`, `.yamllint.yaml`** - Linting configs
```

**Status**: VHDLTest already has this correct (using `.markdownlint-cli2.jsonc`).

### 3.2 CONTRIBUTING.md Updates

#### 3.2.1 Clarify Test Terminology

**Template Commit**: 2757de0 (February 11, 2026)  
**PR**: #24 - "Clarify unit test vs self-validation test terminology in documentation"

**File**: `CONTRIBUTING.md`

The template added clearer distinction between "unit tests" and "self-validation tests". For VHDLTest, the distinction should be between "unit tests" and "integration tests".

Key changes to consider:

1. Update build instructions to say "Run unit tests" instead of just "Run tests"
2. Add a section distinguishing between unit tests and integration tests
3. Update quality check section to be explicit about test types

**Current VHDLTest CONTRIBUTING.md** (lines 30-32):
```markdown
# Run tests
dotnet test
```

**Recommended**:
```markdown
# Run unit tests
dotnet test
```

And in the Testing section (around line 65-71), clarify:
```markdown
### Testing

* All new features should include unit tests
* Integration tests verify the tool works end-to-end with actual VHDL simulators
* Tests use the MSTest framework
* Follow the AAA (Arrange, Act, Assert) pattern
* Test files should be named `[Component]Tests.cs`
* Aim for high test coverage
```

### 3.3 Minor Documentation Fixes

**Template Commit**: d482b9e (February 15, 2026)  
**PR**: #27 - "Fix documentation: remove SARIF references and update examples"

Changes in this commit:
- Removed outdated SARIF-related content from SECURITY.md
- Updated CONTRIBUTING.md examples
- Fixed pull request template

**Action**: Review VHDLTest's SECURITY.md and CONTRIBUTING.md for any outdated references or examples.

---

## 4. Configuration File Updates

### 4.1 Markdownlint Configuration

**Template Commit**: bd14305 (February 10, 2026)  
**PR**: #16 - "Add agent report file exclusions and consolidate markdownlint config"

**File**: `.markdownlint-cli2.jsonc`

**VHDLTest Current**:
```jsonc
{
  "config": {
    "default": true,
    "MD003": { "style": "atx" },
    "MD007": { "indent": 2 },
    "MD013": { "line_length": 120 },
    "MD024": { "siblings_only": true },
    "MD033": false,
    "MD041": false
  },
  "ignores": [
    "node_modules",
    "**/node_modules",
    "AGENT_REPORT_*.md"
  ]
}
```

**Template Current**:
```jsonc
{
  "config": {
    "default": true,
    "MD003": { "style": "atx" },
    "MD007": { "indent": 2 },
    "MD013": { "line_length": 120 },
    "MD033": false,
    "MD041": false
  },
  "ignores": [
    "node_modules",
    "**/AGENT_REPORT_*.md"
  ]
}
```

**Differences**:
1. VHDLTest has `MD024: { "siblings_only": true }` - template doesn't
2. VHDLTest has both `"node_modules"` and `"**/node_modules"` - template has just `"node_modules"`
3. VHDLTest has `"AGENT_REPORT_*.md"` - template has `"**/AGENT_REPORT_*.md"`

**Recommendation**: 
- Keep VHDLTest's `MD024` setting (it's useful)
- Remove redundant `"**/node_modules"` 
- Change `"AGENT_REPORT_*.md"` to `"**/AGENT_REPORT_*.md"` for consistency

**Updated .markdownlint-cli2.jsonc**:
```jsonc
{
  "config": {
    "default": true,
    "MD003": { "style": "atx" },
    "MD007": { "indent": 2 },
    "MD013": { "line_length": 120 },
    "MD024": { "siblings_only": true },
    "MD033": false,
    "MD041": false
  },
  "ignores": [
    "node_modules",
    "**/AGENT_REPORT_*.md"
  ]
}
```

---

## 5. Items NOT Requiring Changes

These are differences between VHDLTest and the template that are **intentional** and should NOT be changed:

### 5.1 Project-Specific Structure

- **Solution file names**: VHDLTest uses `.sln` while template uses `.slnx` - both valid
- **Project names and namespaces**: Project-specific (DEMAConsulting.VHDLTest vs DemaConsulting.TemplateDotNetTool)
- **Repository URLs and badges**: Project-specific

### 5.2 VHDLTest-Specific Features

- **VHDL Simulator Integration**: GHDL, NVC, ModelSim, Vivado integration
- **Test Jobs**: `test-ghdl` and `test-nvc` jobs in workflow
- **MSYS2 Setup**: Windows MSYS2 setup for VHDL simulators
- **Example Tests**: `test/example/` directory with VHDL tests

### 5.3 Different Testing Patterns

- **Template**: Uses self-validation tests via `--validate` flag
- **VHDLTest**: Uses MSTest framework with integration tests
- Both are valid approaches for their respective use cases

### 5.4 .cspell.json Differences

- Project-specific spell check exceptions (e.g., "VHDL", "ghdl", "nvc" in VHDLTest)
- These are expected and should not be synchronized

---

## 6. Implementation Priority and Effort Estimates

### High Priority (Implement Soon)

1. **Tool Version Tracking** (Effort: 4-6 hours)
   - Critical for traceability and regulatory compliance
   - Adds professional polish to build notes
   - Items: 1.1 through 1.8 above

### Medium Priority (Implement When Convenient)

2. **Build Script Updates** (Effort: 15 minutes)
   - Minor clarity improvements
   - Items: 2.1 and 2.2 above

3. **AGENTS.md Updates** (Effort: 30 minutes)
   - Documentation improvements
   - Items: 3.1.1 and maintain general consistency

### Low Priority (Nice to Have)

4. **Markdownlint Config Cleanup** (Effort: 5 minutes)
   - Minor configuration cleanup
   - Item: 4.1 above

5. **CONTRIBUTING.md Clarifications** (Effort: 20 minutes)
   - Minor documentation improvements
   - Item: 3.2 above

---

## 7. Testing Plan

After implementing changes, verify:

### 7.1 Tool Version Tracking

1. Run `dotnet tool restore` to install VersionMark
2. Test version capture locally:
   ```bash
   dotnet versionmark --capture --job-id "test" -- dotnet git node npm
   ```
3. Verify `versionmark-test.json` is created
4. Test version publishing:
   ```bash
   dotnet versionmark --publish --report test-versions.md -- "versionmark-*.json"
   ```
5. Verify the markdown report is generated correctly

### 7.2 Workflow Changes

1. Create a test branch
2. Push changes to trigger CI/CD
3. Verify version capture artifacts are uploaded
4. Verify build-docs job successfully generates versions.md
5. Verify Build Notes PDF includes tool versions section

### 7.3 Build Scripts

1. Run `./build.sh` (Linux/macOS) and `build.bat` (Windows)
2. Verify output messages are clear
3. Verify all tests pass

### 7.4 Documentation

1. Run markdownlint: `npx markdownlint-cli2 "**/*.md"`
2. Verify no new linting errors
3. Verify AGENT_REPORT_*.md files are properly ignored

---

## 8. Summary of Files to Modify

### New Files to Create

1. `.versionmark.yaml` - VersionMark configuration

### Files to Modify

1. `.config/dotnet-tools.json` - Add VersionMark tool
2. `.gitignore` - Add version capture patterns and generated doc files
3. `docs/buildnotes/definition.yaml` - Add versions.md input file
4. `.github/workflows/build.yaml` - Add version capture to all jobs
5. `build.sh` - Update test step message
6. `build.bat` - Update test step message
7. `.markdownlint-cli2.jsonc` - Minor cleanup
8. `AGENTS.md` - Add documentation notes (optional)
9. `CONTRIBUTING.md` - Clarify test terminology (optional)

---

## 9. Questions for Review

Before implementing, consider:

1. **VersionMark Version**: Template uses 0.1.0 - is this the latest stable version?
2. **VHDL Simulator Versions**: Should we capture versions of ModelSim, Vivado, ActiveHDL in addition to GHDL/NVC?
3. **Integration Test Patterns**: How should version capture be added to test-ghdl and test-nvc jobs given their matrix strategy?
4. **Build Notes Structure**: Should tool versions appear at the beginning or end of the build notes?

---

## 10. References

- **Template Repository**: <https://github.com/demaconsulting/TemplateDotNetTool>
- **VersionMark Tool**: <https://github.com/demaconsulting/VersionMark> (likely)
- **Template PR #29**: Tool version tracking
- **Template PR #20**: Build script unit tests
- **Template PR #22**: CHANGELOG.md documentation
- **Template PR #16**: Markdownlint config consolidation

---

**End of Analysis**
