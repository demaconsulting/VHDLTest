# Contributing to VHDLTest

Thank you for your interest in contributing to VHDLTest! This document provides guidelines and information to help you
contribute effectively.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Create a new branch for your feature or bugfix
4. Make your changes
5. Submit a pull request

## Development Environment

### Prerequisites

- .NET SDK 10.0 and 11.0 (both are required: the 11.0 SDK builds the `net11.0` target and
  the .NET 10 runtime runs the `net10.0` tests)
- A code editor (Visual Studio, VS Code, or Rider recommended)

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test
```

## Code Quality Standards

This project maintains high code quality through automated analysis and testing.

### Code Style

- Follow C# naming conventions (PascalCase for public members, camelCase for local variables)
- Private fields should use `_camelCase` naming
- Use `var` only when the type is obvious from the right side
- Prefer explicit types when they improve readability
- Use file-scoped namespaces
- Include XML documentation for public APIs

### EditorConfig

The project includes an `.editorconfig` file that configures code formatting rules. Most modern editors will
automatically apply these rules. Key settings include:

- Indent with 4 spaces
- Use CRLF line endings on Windows, LF on Unix
- Insert final newline
- Trim trailing whitespace

### Code Analyzers

The project uses:

- **Microsoft.CodeAnalysis.NetAnalyzers** - Provides performance, security, and reliability analysis

Run `dotnet build` to see analyzer feedback.

### Testing

- All new features should include unit tests
- Tests use the MSTest framework
- Follow the AAA (Arrange, Act, Assert) pattern
- Test files should be named `[Component]Tests.cs`
- Aim for high test coverage

### Help Cards

The built-in help (`vhdltest help`) is served from Markdown help cards in
`src/DEMAConsulting.VHDLTest/Help/`, which the DemaConsulting.HelpCards package embeds in the tool
assembly. `Cli/HelpCommand.cs` is the only code that uses the package.

- **One file per topic.** The path below `Help/`, without `.md`, is the card id; the cards under
  `Help/simulators/` are the child topics of `simulators`. `_source.md` names the source (`name:`) and sets
  `schema: 2`.
- **Front matter.** Every card has a `title`, a `summary` (at most 80 characters for a topic, 100 otherwise), and
  exactly one purpose tag: `tutorial`, `reference`, or `troubleshooting`. Add `aliases` for topic names that users
  commonly guess, such as `examples`, so `help <name>` finds the card.
- **New topics.** Add the id of every new top-level card to `HelpCommand.RootCardIds`, the one list of topics; a
  missing or extra topic fails the tests.
- **Examples.** Give every task a runnable `command` example, one command per example: no pipes, redirection, or
  `&&`. Wrap `content` in single quotes and use double quotes inside the command, which bash, PowerShell, and cmd
  read alike. State the result when it matters with `exit:` (default 0) and `expect:`, a short fragment that must
  appear somewhere in the console output; `expect` cannot check a file that the command writes. Keep each
  `description` to about 50 characters.
- **Simulators.** No VHDL simulator is installed where the examples run, and CI installs NVC, so a runnable
  example must never depend on which simulators are installed: every command that runs test benches or
  `--validate` names `--simulator mock`. Commands that need a real simulator are `snippet` examples.
- **Getting started.** Say how to install the tool for CI: the .NET version it needs (from `TargetFrameworks`), a
  local tool manifest, and the global install as the alternative. The CI example must be a complete GitHub Actions
  steps snippet (`language: yaml`) with `actions/setup-dotnet` for that .NET version, a simulator install,
  `dotnet tool restore`, and a `dotnet vhdltest` step that runs the test benches; prose alone is not enough. Mark
  the install and CI examples as `snippet` examples so that the example runner does not run them.
- **Input files.** Show complete files only for formats that users author, and describe formats that the tool
  produces (TRX, JUnit) in prose. The examples read `tests.yaml` and `misspelled.yaml` from
  `test/DEMAConsulting.VHDLTest.Tests/TestData/HelpExamples/`, which the example-runner test copies into
  `invocation.WorkingDirectory` before running the tool. Keep `tests.yaml` identical to the `tests` example of the
  `configuration` card.
- **Width.** A printed card may not be wider than 120 characters. Links print as full commands (`[[#name]]` as
  about 50 characters) and `{{version}}` of a development build as about 46, so keep body lines near 75 characters
  with at most one link each.
- **Text.** Cards are linted and spell-checked like any Markdown file: no body line starting with `#`, no wide
  tables, and every fenced block names its language.

Run `pwsh ./build.ps1 -Changed` after changing a card. The `HelpCommand` unit tests validate every card, with
warnings treated as errors, and check its printed width; an integration test runs every command example against
the built tool and compares the result with the card.

### Requirements

This project maintains formal requirements documentation to ensure quality, traceability, and regulatory compliance
support.

#### Requirements Structure

- **requirements.yaml**: Master requirements document in the project root
- **docs/requirements_doc/**: Additional requirements documentation
- **docs/requirements_report/**: Requirements traceability matrix

#### Working with Requirements

When contributing to this project:

1. **Review Applicable Requirements**: Before making changes, review `requirements.yaml` for relevant requirements
2. **Maintain Traceability**: If you add new features:
   - Add new requirements to `requirements.yaml` with unique IDs (e.g., `CLI-001`, `TEST-001`)
   - Link requirements to test cases that verify them
   - Prefer linking to integration tests (tests starting with `IntegrationTest_`)
3. **Update Tests**: Ensure all requirements have associated test coverage
4. **Verify Compliance**: Run the validation command to check requirements coverage:

   ```bash
   dotnet vhdltest --validate
   ```

5. **Update Documentation**: If you add new requirement categories, update `docs/requirements_doc/introduction.md`

#### Requirement Categories

Requirements are organized into categories with prefixed IDs:

- **CLI-***: Command-line interface and user interaction
- **TEST-***: Test execution and result handling
- **VAL-***: Self-validation and tool verification
- **SIM-***: HDL simulator integration
- **PLT-***: Platform and runtime support

### Pull Request Guidelines

1. **Keep changes focused** - One feature or fix per PR
2. **Write clear commit messages** - Explain what and why
3. **Include tests** - New features need test coverage
4. **Update requirements** - Add or update requirements in `requirements.yaml` for new features
5. **Update documentation** - Keep README and comments current
6. **Follow code style** - Respect the existing patterns
7. **Ensure CI passes** - All checks must pass before merging

### Commit Messages

Use clear, descriptive commit messages:

```text
Add support for additional VHDL simulators

- Implement XYZ simulator interface
- Add unit tests for XYZ simulator
- Update documentation
```

## Code Review Process

All submissions require review before merging. Reviewers will check:

- Code quality and style
- Test coverage
- Requirements traceability
- Documentation updates
- Backward compatibility
- Performance implications

## Questions?

- Open an issue for bugs or feature requests
- Start a discussion for general questions
- Review existing issues before creating new ones

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
