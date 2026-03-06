# Agent Quick Reference

Project-specific guidance for agents working on VHDLTest - a .NET CLI tool for running VHDL unit tests and
generating test reports.

## Available Specialized Agents

- **Requirements Agent** - Develops requirements and ensures test coverage linkage
- **Technical Writer** - Creates accurate documentation following regulatory best practices
- **Software Developer** - Writes production code and self-validation tests in literate style
- **Test Developer** - Creates unit and integration tests following AAA pattern
- **Code Quality Agent** - Enforces linting, static analysis, and security standards
- **Repo Consistency Agent** - Ensures VHDLTest remains consistent with TemplateDotNetTool template patterns

## Agent Selection Guide

- Fix a bug → **Software Developer**
- Add a new feature → **Requirements Agent** → **Software Developer** → **Test Developer**
- Write a test → **Test Developer**
- Fix linting or static analysis issues → **Code Quality Agent**
- Update documentation → **Technical Writer**
- Add or update requirements → **Requirements Agent**
- Ensure test coverage linkage in `requirements.yaml` → **Requirements Agent**
- Run security scanning or address CodeQL alerts → **Code Quality Agent**
- Propagate template changes → **Repo Consistency Agent**

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet
- HDL Simulators: GHDL, ModelSim, Vivado, ActiveHDL, NVC

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.json`, `.markdownlint-cli2.jsonc`, `.yamllint.yaml`** - Linting configs

## Requirements (VHDLTest-Specific)

- Link ALL requirements to tests (prefer `IntegrationTest_*` tests over unit tests)
- Not all tests need to be linked to requirements (tests may exist for corner cases, design testing, failure-testing, etc.)
- Enforced in CI: `dotnet reqstream --requirements requirements.yaml --tests "artifacts/**/*.trx" --enforce`
- When adding features: add requirement + link to test

## Test Source Filters

Test links in `requirements.yaml` can include a source filter prefix to restrict which test results count as
evidence. This is critical for platform, simulator, and framework requirements - **do not remove these filters**.

- `ghdl@TestName` - proves the test passed using the GHDL simulator
- `nvc@TestName` - proves the test passed using the NVC simulator
- `windows@TestName` - proves the test passed on a Windows platform
- `ubuntu@TestName` - proves the test passed on a Linux (Ubuntu) platform
- `dotnet8.x@TestName` - proves the self-validation test ran on a machine with .NET 8.x runtime
- `dotnet9.x@TestName` - proves the self-validation test ran on a machine with .NET 9.x runtime
- `dotnet10.x@TestName` - proves the self-validation test ran on a machine with .NET 10.x runtime

Without the source filter, a test result from any platform/simulator/framework satisfies the requirement. Adding
the filter ensures the CI evidence comes specifically from the required environment.

## Testing (VHDLTest-Specific)

- **Test Naming**: `ClassName_MethodUnderTest_Scenario_ExpectedBehavior` (unit tests) or
  `IntegrationTest_Scenario_ExpectedBehavior` (integration tests)
- **Self-Validation**: Tests run via `--validate` flag and can output TRX/JUnit format
- **MSTest v4**: Use modern assertions like `Assert.IsTrue()`, `Assert.AreEqual()`
- **Integration Tests**: Prefer tests that run the actual VHDLTest tool (starting with `IntegrationTest_`)

## Code Style (VHDLTest-Specific)

- **XML Docs**: On ALL members (public/internal/private) with spaces after `///`
- **Errors**: `ArgumentException` for parsing, `InvalidOperationException` for runtime issues
- **Namespace**: File-scoped namespaces only
- **Using Statements**: Top of file only
- **Private fields**: Prefix with underscore (`_fieldName`)
- **String Formatting**: Use interpolated strings ($"") for clarity
- **Warnings**: Zero warnings required (`TreatWarningsAsErrors=true`)

## Project Structure

- **Context.cs**: Handles command-line argument parsing, logging, and output
- **Program.cs**: Main entry point with version/help/validation routing
- **Validation.cs**: Self-validation tests with TRX/JUnit output support
- **Simulators/**: VHDL simulator integration (GHDL, ModelSim, Vivado, ActiveHDL, NVC)

## Build and Test

```bash
# Build the project
dotnet build --configuration Release

# Run unit tests
dotnet test --configuration Release

# Run self-validation
dotnet run --project src/DEMAConsulting.VHDLTest \
  --configuration Release --framework net10.0 --no-build -- --validate

# Use convenience scripts
./build.sh    # Linux/macOS
build.bat     # Windows
```

## Documentation

- **User Guide**: `docs/guide/guide.md`
- **Requirements**: `requirements.yaml` → auto-generated docs
- **Build Notes**: Auto-generated via BuildMark
- **Code Quality**: Auto-generated via CodeQL and SonarMark
- **Trace Matrix**: Auto-generated via ReqStream
- **CHANGELOG.md**: Not present - changes are captured in the auto-generated build notes

## Markdown Link Style

- **AI agent markdown files** (`.github/agents/*.md`): Use inline links `[text](url)` so URLs are visible in
  agent context
- **README.md**: Use absolute URLs (shipped in NuGet package)
- **All other markdown files**: Use reference-style links `[text][ref]` with `[ref]: url` at document end

## CI/CD

- **Quality Checks**: Markdown lint, spell check, YAML lint
- **Build**: Multi-platform (Windows/Linux)
- **CodeQL**: Security scanning
- **VHDL Simulation Tests**: GHDL and NVC on Windows/Linux
- **Documentation**: Auto-generated via Pandoc + Weasyprint

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
# 5. Requirements: dotnet reqstream --requirements requirements.yaml --tests "artifacts/**/*.trx" --enforce
```

## Agent Report Files

When agents need to write report files to communicate with each other or the user, follow these guidelines:

- **Naming Convention**: Use the pattern `AGENT_REPORT_xxxx.md` (e.g., `AGENT_REPORT_analysis.md`)
- **Purpose**: These files are for temporary inter-agent communication and should not be committed
- **Exclusions**: Files matching `AGENT_REPORT_*.md` are automatically:
  - Excluded from git (via .gitignore)
  - Excluded from markdown linting
  - Excluded from spell checking

## Custom Agents

Delegate tasks to specialized agents for better results:

- **requirements-agent** - Invoke for: creating/reviewing requirements, test coverage strategy
- **technical-writer** - Invoke for: documentation updates/reviews, markdown/spell/YAML linting
- **software-developer** - Invoke for: production code implementation, refactoring for testability
- **test-developer** - Invoke for: unit and integration tests, test coverage improvements
- **code-quality-agent** - Invoke for: code quality reviews, linting, static analysis, security, zero-warning builds
- **repo-consistency-agent** - Invoke for: checking consistency with TemplateDotNetTool template patterns
