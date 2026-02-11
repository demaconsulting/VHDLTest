# Agent Quick Reference

Project-specific guidance for agents working on VHDLTest - a .NET CLI tool for running VHDL unit tests and
generating test reports.

## Available Specialized Agents

- **Requirements Agent** - Develops requirements and ensures test coverage linkage
- **Technical Writer** - Creates accurate documentation following regulatory best practices
- **Software Developer** - Writes production code in literate style
- **Test Developer** - Creates unit and integration tests following AAA pattern
- **Code Quality Agent** - Enforces linting, static analysis, and security standards
- **Repo Consistency Agent** - Ensures VHDLTest remains consistent with TemplateDotNetTool template patterns

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

- **XML Docs**: On ALL members (public/internal/private) with spaces after `///`
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

- **requirements-agent** - Invoke for: creating/reviewing requirements, test coverage strategy
- **technical-writer** - Invoke for: documentation updates/reviews, markdown/spell/YAML linting
- **software-developer** - Invoke for: production code implementation, refactoring for testability
- **test-developer** - Invoke for: unit and integration tests, test coverage improvements
- **code-quality-agent** - Invoke for: code quality reviews, linting, static analysis, security, zero-warning builds
- **repo-consistency-agent** - Invoke for: checking consistency with TemplateDotNetTool template patterns
