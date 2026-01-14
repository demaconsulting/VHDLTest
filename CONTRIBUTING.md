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

* .NET SDK 8.0, 9.0, or 10.0
* A code editor (Visual Studio, VS Code, or Rider recommended)

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

* Follow C# naming conventions (PascalCase for public members, camelCase for local variables)
* Private fields should use `_camelCase` naming
* Use `var` only when the type is obvious from the right side
* Prefer explicit types when they improve readability
* Use file-scoped namespaces
* Include XML documentation for public APIs

### EditorConfig

The project includes an `.editorconfig` file that configures code formatting rules. Most modern editors will
automatically apply these rules. Key settings include:

* Indent with 4 spaces
* Use CRLF line endings on Windows, LF on Unix
* Insert final newline
* Trim trailing whitespace

### Code Analyzers

The project uses:

* **Microsoft.CodeAnalysis.NetAnalyzers** - Provides performance, security, and reliability analysis

Run `dotnet build` to see analyzer feedback.

### Testing

* All new features should include unit tests
* Tests use the MSTest framework
* Follow the AAA (Arrange, Act, Assert) pattern
* Test files should be named `[Component]Tests.cs`
* Aim for high test coverage

### Requirements

This project maintains formal requirements documentation to ensure quality, traceability, and regulatory compliance support.

#### Requirements Structure

* **requirements.yaml**: Master requirements document in the project root
* **docs/requirements/**: Additional requirements documentation
* **docs/tracematrix/**: Requirements traceability matrix

#### Working with Requirements

When contributing to this project:

1. **Review Applicable Requirements**: Check `requirements.yaml` for requirements related to your changes
2. **Maintain Traceability**: If you add new features:
   * Add new requirements to `requirements.yaml` with unique IDs (e.g., `CLI-001`, `TEST-001`)
   * Link requirements to test cases that verify them
   * Prefer linking to integration tests (tests starting with `IntegrationTest_`)
3. **Update Tests**: Ensure all requirements have associated test coverage
4. **Verify Compliance**: Run the validation command to check requirements coverage:

   ```bash
   dotnet vhdltest --validate
   ```

5. **Update Documentation**: If you add new requirement categories, update `docs/requirements/introduction.md`

#### Requirement Categories

Requirements are organized into categories with prefixed IDs:

* **CLI-***: Command-line interface and user interaction
* **TEST-***: Test execution and result handling
* **VAL-***: Self-validation and tool verification
* **SIM-***: HDL simulator integration
* **PLT-***: Platform and runtime support

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

* Code quality and style
* Test coverage
* Requirements traceability
* Documentation updates
* Backward compatibility
* Performance implications

## Questions?

* Open an issue for bugs or feature requests
* Start a discussion for general questions
* Review existing issues before creating new ones

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
