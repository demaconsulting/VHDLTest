# AI Instructions for VHDLTest

This file provides specific context and instructions for AI coding agents to
interact effectively with this C# project.


## Project Overview

VHDLTest is a C# .NET tool for running VHDL unit tests and generating test reports.


## Technologies and Dependencies

* **Language**: C# 12
* **.NET Frameworks**: .NET 8, 9, and 10
* **Primary Dependencies**: [YamlDotNet]
* **HDL Simulators**: [GHDL, ModelSim, Vivado, ActiveHDL, NVC]


## Project Structure

The repository is organized as follows:

* `/.config/`: Contains the .NET Tool configuration.
* `/.github/workflows/`: Contains the CI/CD pipeline configurations.
* `/src/DEMAConsulting.VHDLTest/`: Contains the tool source code.
* `/test/DEMAConsulting.VHDLTest.Tests/`: Contains the tool unit tests.
* `/test/example/`: Contains example VHDL modules and unit tests.
* `/DEMAConsulting.VHDLTest.sln`: The main Visual Studio solution file.


## Development Commands

Use these commands to perform common development tasks:

* **Restore DotNet Tools**:
  ```bash
  dotnet tool restore
  ```

* **Build the Project**:
  ```bash
  dotnet build
  ```

* **Run All Tests**:
  ```bash
  dotnet test
  ```


## Testing Guidelines

* Tests are located under the `/test/DEMAConsulting.VHDLTest.Tests/` folder and use the MSTest framework.
* Test files should end with `.cs` and adhere to the naming convention `[Component]Tests.cs`.
* All new features should be tested with comprehensive unit tests.
* The build must pass all tests and static analysis warnings before merging.
* Tests should be written using the AAA (Arrange, Act, Assert) pattern.


## Code Style and Conventions

* Follow standard C# naming conventions (PascalCase for classes/methods/properties, camelCase for local variables).
* Use nullable reference types (`#nullable enable`).
* Warnings are treated as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`).
* Avoid public fields; prefer properties.
* Follow the formatting guidelines in `.editorconfig`.
* Private fields should be prefixed with underscore (`_fieldName`).


## Code Quality and Analysis

The project uses several tools to maintain code quality:

* **EditorConfig** (`.editorconfig`): Defines consistent coding styles across editors and IDEs.
* **StyleCop.Analyzers**: Enforces C# style and consistency rules.
* **Microsoft.CodeAnalysis.NetAnalyzers**: Provides additional static analysis for performance, security, and reliability.
* **Directory.Build.props**: Centralizes build configuration and analyzer settings across all projects.
* **VHDLTest.ruleset**: Configures which analyzer rules are enforced as errors, warnings, or informational messages.
* **stylecop.json**: Customizes StyleCop analyzer behavior.

### Running Code Analysis

Code analysis runs automatically during build:

```bash
dotnet build
```

Many code quality issues are reported as informational messages to guide improvements without blocking the build. Critical issues are reported as errors.

### Quality Tools in CI/CD

The CI/CD pipeline includes:
* SonarCloud analysis for code quality metrics
* Code coverage reporting with Coverlet
* SBOM (Software Bill of Materials) generation
* Security scanning


## Boundaries and Guardrails

* **NEVER** modify files within the `/obj/` or `/bin/` directories.
* **NEVER** commit secrets, API keys, or sensitive configuration data.
* **ASK FIRST** before making significant architectural changes to the core library logic.
