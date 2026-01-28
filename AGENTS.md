# Agent Quick Reference

Project-specific guidance for agents working on VHDLTest - a .NET CLI tool for running VHDL unit tests and
generating test reports.

## Tech Stack

- C# 12, .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet
- HDL Simulators: GHDL, ModelSim, Vivado, ActiveHDL, NVC

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet vhdltest --validate`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8+BOM, LF endings)
- **`.cspell.json`, `.markdownlint.json`, `.yamllint.yaml`** - Linting configs

## Requirements (VHDLTest-Specific)

- Link ALL requirements to tests (prefer `IntegrationTest_*` tests over unit tests)
- Enforced: `dotnet vhdltest --validate` generates requirements traceability matrix
- When adding features: add requirement + link to test

## Testing (VHDLTest-Specific)

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` (unit tests) or
  `IntegrationTest_Scenario_ExpectedBehavior` (integration tests)
- **MSTest v4**: Use modern assertions like `Assert.IsTrue()`, `Assert.AreEqual()`
- **Integration Tests**: Prefer tests that run the actual VHDLTest tool (starting with `IntegrationTest_`)

## Code Style (VHDLTest-Specific)

- **XML Docs**: On public members with spaces after `///`
- **Private fields**: Prefix with underscore (`_fieldName`)
- **Warnings**: Zero warnings required (`TreatWarningsAsErrors=true`)

## Build & Quality (Quick Reference)

```bash
# Standard build/test
dotnet build --configuration Release && dotnet test --configuration Release

# Helper scripts (cross-platform)
./build.sh    # or build.bat on Windows
./lint.sh     # or lint.bat on Windows

# Pre-finalization checklist (in order):
# 1. Build/test (zero warnings required)
# 2. code_review tool
# 3. codeql_checker tool
# 4. All linters (markdownlint, cspell, yamllint)
# 5. Requirements: dotnet vhdltest --validate
```

## Custom Agents

Delegate tasks to specialized agents for better results:

- **documentation-writer** - Invoke for: documentation updates/reviews, requirements.yaml changes,
  markdown/spell/YAML linting
- **project-maintainer** - Invoke for: dependency updates, CI/CD maintenance, releases, requirements traceability enforcement
- **software-quality-enforcer** - Invoke for: code quality reviews, test coverage verification, static analysis, zero-warning builds, requirements test quality
