# Detailed File-by-File Comparison

## Files Requiring Updates

### 1. `.config/dotnet-tools.json`

**Status**: Missing VersionMark tool

**Template Has**:
```json
"demaconsulting.versionmark": {
  "version": "0.1.0",
  "commands": ["versionmark"]
}
```

**VHDLTest Needs**: Add this tool entry

---

### 2. `.versionmark.yaml`

**Status**: File does not exist in VHDLTest

**Template Has**: Complete configuration file with tool definitions

**VHDLTest Needs**: Create new file (see TEMPLATE_CONSISTENCY_ANALYSIS.md for full content)

**VHDLTest-Specific Additions**:
- Add GHDL version capture
- Add NVC version capture

---

### 3. `.gitignore`

**Status**: Missing several entries

**Template Has**:
```gitignore
# Generated documentation
docs/requirements/requirements.md
docs/justifications/justifications.md
docs/tracematrix/tracematrix.md
docs/quality/codeql-quality.md
docs/quality/sonar-quality.md
docs/buildnotes.md
docs/buildnotes/versions.md

# VersionMark captures
versionmark-*.json

# Agent report files
AGENT_REPORT_*.md
```

**VHDLTest Has**:
```gitignore
# Limited generated documentation entries
docs/guide/guide.html
docs/requirements/requirements.md
docs/requirements/requirements.html
# ... partial list

AGENT_REPORT_*.md
```

**VHDLTest Needs**: Add missing entries, especially:
- `docs/buildnotes/versions.md`
- `versionmark-*.json`
- More complete generated docs list

---

### 4. `docs/buildnotes/definition.yaml`

**Status**: Missing versions.md input file

**Template Has**:
```yaml
input-files:
  - docs/buildnotes/title.txt
  - docs/buildnotes/introduction.md
  - docs/buildnotes.md
  - docs/buildnotes/versions.md  # THIS LINE
```

**VHDLTest Has**:
```yaml
input-files:
  - docs/buildnotes/title.txt
  - docs/buildnotes/introduction.md
  - docs/buildnotes.md
  # Missing versions.md
```

**VHDLTest Needs**: Add `docs/buildnotes/versions.md` line

---

### 5. `.github/workflows/build.yaml`

**Status**: Missing version capture steps in all jobs

#### 5a. quality-checks Job

**Template Has**: 
- Setup dotnet step
- Restore tools step
- Capture tool versions step (NEW)
- Upload version capture artifact (NEW)

**VHDLTest Has**:
- No dotnet setup in quality-checks
- No tool restore
- No version capture

**VHDLTest Needs**: Add 4 new steps (setup, restore, capture, upload)

---

#### 5b. build Job

**Template Has**:
```yaml
- name: Capture tool versions
  shell: bash
  run: |
    OS_SHORT=$(echo "${{ matrix.os }}" | sed 's/windows-latest/win/;s/ubuntu-latest/ubuntu/')
    JOB_ID="build-${OS_SHORT}"
    dotnet versionmark --capture --job-id "${JOB_ID}" -- dotnet git dotnet-sonarscanner versionmark

- name: Upload version capture
  uses: actions/upload-artifact@v6
  with:
    name: version-capture-${{ matrix.os }}
    path: versionmark-build-*.json
```

**VHDLTest Has**: No version capture steps

**VHDLTest Needs**: Add 2 new steps (capture, upload)

---

#### 5c. Integration Test Jobs (test-ghdl, test-nvc)

**Template Pattern**: Integration test jobs check out `.versionmark.yaml` and capture versions

**VHDLTest Has**: No checkout of config files, no version capture

**VHDLTest Needs**: 
- Add sparse checkout step at beginning
- Add version capture step before test results upload
- Add version capture artifact upload

---

#### 5d. build-docs Job

**Template Has**:
```yaml
# After downloading CodeQL SARIF:
- name: Download all version captures
  uses: actions/download-artifact@v7
  with:
    path: version-captures
    pattern: 'version-capture-*'

# After restoring tools:
- name: Capture tool versions for build-docs
  shell: bash
  run: |
    dotnet versionmark --capture --job-id "build-docs" -- \
      dotnet git node npm pandoc weasyprint sarifmark sonarmark reqstream buildmark versionmark

# After BuildMark report generation:
- name: Publish Tool Versions
  shell: bash
  run: |
    dotnet versionmark --publish --report docs/buildnotes/versions.md --report-depth 1 \
      -- "versionmark-*.json" "version-captures/**/versionmark-*.json"

- name: Display Tool Versions Report
  shell: bash
  run: |
    cat docs/buildnotes/versions.md
```

**VHDLTest Has**: None of these steps

**VHDLTest Needs**: Add 4 new steps (download, capture, publish, display)

---

### 6. `build.sh`

**Status**: Minor messaging update

**Template Has**:
```bash
echo "🧪 Running unit tests..."
```

**VHDLTest Has**:
```bash
echo "✅ Running tests..."
```

**VHDLTest Needs**: Change to "🧪 Running unit tests..."

---

### 7. `build.bat`

**Status**: Minor messaging update

**Template Has**:
```batch
echo Running unit tests...
```

**VHDLTest Has**:
```batch
echo Running tests...
```

**VHDLTest Needs**: Change to "Running unit tests..."

---

### 8. `.markdownlint-cli2.jsonc`

**Status**: Minor pattern update

**Template Has**:
```jsonc
"ignores": [
  "node_modules",
  "**/AGENT_REPORT_*.md"
]
```

**VHDLTest Has**:
```jsonc
"ignores": [
  "node_modules",
  "**/node_modules",
  "AGENT_REPORT_*.md"
]
```

**VHDLTest Needs**: 
- Remove redundant `"**/node_modules"`
- Change to `"**/AGENT_REPORT_*.md"` for better pattern matching

---

### 9. `AGENTS.md` (Optional)

**Status**: Missing some documentation notes

**Template Has**:
```markdown
## Documentation

- **CHANGELOG.md**: Not present - changes are captured in the auto-generated build notes
```

**VHDLTest Has**: No explicit "Documentation" section

**VHDLTest Needs**: Consider adding documentation section with CHANGELOG.md note

---

### 10. `CONTRIBUTING.md` (Optional)

**Status**: Could be more explicit about test types

**Template Has**:
- Separate sections for "Unit Tests" and "Self-Validation Tests"
- Clear distinction in build instructions

**VHDLTest Has**:
- Generic "Testing" section
- No explicit distinction between unit and integration tests

**VHDLTest Needs**: 
- Update "Run tests" to "Run unit tests"
- Consider adding subsections for unit vs integration tests

---

## Summary Statistics

- **Critical Files**: 5 (dotnet-tools.json, .versionmark.yaml, .gitignore, buildnotes definition, workflow)
- **Workflow Jobs Affected**: 5 (quality-checks, build, test-ghdl, test-nvc, build-docs)
- **New Workflow Steps**: ~15-20 new steps across all jobs
- **Documentation Files**: 4 (build.sh, build.bat, AGENTS.md, CONTRIBUTING.md)
- **Config Files**: 2 (.gitignore, .markdownlint-cli2.jsonc)

---

## Implementation Order Recommendation

1. **Phase 1 - Local Configuration** (30 min)
   - Add VersionMark to dotnet-tools.json
   - Create .versionmark.yaml
   - Update .gitignore
   - Update buildnotes definition.yaml
   - Run `dotnet tool restore` and test locally

2. **Phase 2 - Workflow Updates** (2-3 hours)
   - Update quality-checks job
   - Update build job
   - Update test-ghdl job
   - Update test-nvc job
   - Update build-docs job

3. **Phase 3 - Testing** (1 hour)
   - Push to test branch
   - Verify all workflow jobs complete
   - Verify version captures are uploaded
   - Verify build notes include versions
   - Download and review Build Notes PDF

4. **Phase 4 - Cleanup** (30 min)
   - Update build scripts (build.sh, build.bat)
   - Update markdownlint config
   - Update AGENTS.md (optional)
   - Update CONTRIBUTING.md (optional)

**Total Estimated Time**: 4-6 hours

---
