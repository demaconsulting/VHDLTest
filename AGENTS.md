# Agent Quick Reference

Project-specific guidance for agents working on VHDLTest - a .NET CLI tool for running VHDL unit tests and
generating test reports.

## Standards Application (ALL Agents Must Follow)

Before performing any work, agents must read and apply the relevant standards from `.github/standards/`:

- **`csharp-language.md`** - For C# code development (literate programming, XML docs, dependency injection)
- **`csharp-testing.md`** - For C# test development (AAA pattern, naming, MSTest anti-patterns)
- **`reqstream-usage.md`** - For requirements management (traceability, semantic IDs, source filters)
- **`reviewmark-usage.md`** - For file review management (review-sets, file patterns, enforcement)
- **`software-items.md`** - For software categorization (system/subsystem/unit/OTS classification)
- **`technical-documentation.md`** - For documentation creation and maintenance (structure, Pandoc, README best practices)

Load only the standards relevant to your specific task scope and apply their
quality checks and guidelines throughout your work.

## Agent Delegation Guidelines

The default agent should handle simple, straightforward tasks directly.
Delegate to specialized agents only for specific scenarios:

- **Light development work** (small fixes, simple features) → Call @developer agent
- **Light quality checking** (linting, basic validation) → Call @quality agent
- **Formal feature implementation** (complex, multi-step) → Call the `@implementation` agent
- **Formal bug resolution** (complex debugging, systematic fixes) → Call the `@implementation` agent
- **Formal reviews** (compliance verification, detailed analysis) → Call @code-review agent
- **Template consistency** (downstream repository alignment) → Call @repo-consistency agent

## Available Specialized Agents

- **code-review** - Agent for performing formal reviews using standardized
  review processes
- **developer** - General-purpose software development agent that applies
  appropriate standards based on the work being performed
- **implementation** - Orchestrator agent that manages quality implementations
  through a formal state machine workflow
- **quality** - Quality assurance agent that grades developer work against DEMA
  Consulting standards and Continuous Compliance practices
- **repo-consistency** - Ensures VHDLTest remains consistent with
  TemplateDotNetTool template patterns and best practices

## Agent Selection Guide

- Fix a bug → **software-developer**
- Add a new feature → **requirements** → **software-developer** → **test-developer**
- Write a test → **test-developer**
- Fix linting or static analysis issues → **code-quality**
- Update documentation → **technical-writer**
- Add or update requirements → **requirements**
- Ensure test coverage linkage in `requirements.yaml` → **requirements**
- Run security scanning or address CodeQL alerts → **code-quality**
- Perform formal file reviews → **code-review**
- Propagate template changes → **repo-consistency**

## Tech Stack

- C# (latest), .NET 8.0/9.0/10.0, MSTest, dotnet CLI, NuGet
- HDL Simulators: GHDL, ModelSim, Vivado, ActiveHDL, NVC

## Key Files

- **`requirements.yaml`** - All requirements with test linkage (enforced via `dotnet reqstream --enforce`)
- **`.editorconfig`** - Code style (file-scoped namespaces, 4-space indent, UTF-8, LF endings)
- **`.cspell.yaml`, `.markdownlint-cli2.yaml`, `.yamllint.yaml`** - Linting configs

### Spell Check Word List Policy

**Never** add a word to the `.cspell.yaml` word list in order to silence a spell-checking failure.
Doing so defeats the purpose of spell-checking and reduces the quality of the repository.

- If cspell flags a word that is **misspelled**, fix the spelling in the source file.
- If cspell flags a word that is a **genuine technical term** (tool name, project identifier, etc.) and is
  spelled correctly, raise a **proposal** (e.g. comment in a pull request) explaining why the word
  should be added. The proposal must be reviewed and approved before the word is added to the list.

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
- `macos@TestName` - proves the test passed on a macOS platform
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

- **AI agent markdown files** (`.github/agents/*.agent.md`): Use inline links `[text](url)` so URLs are visible in
  agent context
- **README.md**: Use absolute URLs (shipped in NuGet package)
- **All other markdown files**: Use reference-style links `[text][ref]` with `[ref]: url` at document end

## CI/CD

- **Quality Checks**: Markdown lint, spell check, YAML lint
- **Build**: Multi-platform (Windows/Linux/macOS)
- **CodeQL**: Security scanning
- **VHDL Simulation Tests**: GHDL on Windows/Linux/macOS, NVC on Windows/Linux/macOS
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

- **requirements** - Invoke for: creating/reviewing requirements, test coverage strategy
- **technical-writer** - Invoke for: documentation updates/reviews, markdown/spell/YAML linting
- **software-developer** - Invoke for: production code implementation, refactoring for testability
- **test-developer** - Invoke for: unit and integration tests, test coverage improvements
- **code-quality** - Invoke for: code quality reviews, linting, static analysis, security, zero-warning builds
- **code-review** - Invoke for: performing formal file reviews and compliance verification
- **repo-consistency** - Invoke for: checking consistency with TemplateDotNetTool template patterns
- **developer** - Invoke for: light development work applying appropriate standards
- **implementation** - Invoke for: complex multi-step feature implementation or bug resolution
- **quality** - Invoke for: grading developer work against DEMA Consulting standards
