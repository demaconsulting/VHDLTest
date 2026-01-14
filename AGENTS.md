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

```text
VHDLTest/
├── .config/                        # .NET Tool configuration
├── .github/
│   └── workflows/                  # CI/CD pipeline configurations
├── src/
│   └── DEMAConsulting.VHDLTest/    # Tool source code
├── test/
│   ├── DEMAConsulting.VHDLTest.Tests/  # Tool unit tests
│   └── example/                    # Example VHDL modules and unit tests
└── DEMAConsulting.VHDLTest.sln     # Main Visual Studio solution file
```

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
* Test methods must follow these naming conventions:
  * **Unit tests** (testing specific classes/methods): `ClassName_MethodUnderTest_Scenario_ExpectedBehavior`
    * Example: `Context_Create_NoArguments_ReturnsDefaultContext`
    * Example: `SimulatorFactory_Get_GhdlSimulator_ReturnsGhdlSimulator`
    * Example: `Options_Parse_NoConfigProvided_ThrowsInvalidOperationException`
  * **Integration tests** (running the actual tool): `IntegrationTest_Scenario_ExpectedBehavior`
    * Example: `IntegrationTest_ValidateFlag_PerformsValidationAndReturnsSuccess`
    * Example: `IntegrationTest_TestsPassed_ReturnsZeroExitCode`
    * Example: `IntegrationTest_HelpShortFlag_DisplaysUsageAndReturnsSuccess`
* Requirements should primarily link to integration tests that actually run the VHDLTest tool (tests starting with `IntegrationTest_`).

## Code Style and Conventions

* Follow standard C# naming conventions (PascalCase for classes/methods/properties, camelCase for local variables).
* Use nullable reference types (`<Nullable>enable</Nullable>` in csproj files).
* Warnings are treated as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`).
* Avoid public fields; prefer properties.
* Follow the formatting guidelines in `.editorconfig`.
* Private fields should be prefixed with underscore (`_fieldName`).

## Code Quality and Analysis

The project uses several tools to maintain code quality:

* **EditorConfig** (`.editorconfig`): Defines consistent coding styles across editors and IDEs.
* **Microsoft.CodeAnalysis.NetAnalyzers**: Provides static analysis for performance, security, and reliability.
* **SonarAnalyzer.CSharp**: Additional static analysis for code quality and security.
* **.cspell.json**: Spell checking configuration with project-specific dictionary.
* **.markdownlint.json**: Markdown linting rules configuration.
* **.yamllint.yaml**: YAML linting configuration for maintaining consistent YAML file formatting.

### Running Code Analysis

Code analysis runs automatically during build:

```bash
dotnet build
```

### Quality Tools in CI/CD

The CI/CD pipeline includes:

* Quality Checks (spell checking, markdown linting, YAML linting)
* SonarCloud analysis for code quality metrics
* Code coverage reporting with Coverlet
* SBOM (Software Bill of Materials) generation
* Security scanning
* Dependabot for weekly dependency updates

### Running Quality Checks

You can run the quality checks locally to ensure your changes meet project standards:

* **Spell Check**:

  ```bash
  npx cspell "**/*.md" "**/*.cs" "**/*.yaml" "**/*.yml" --config .cspell.json
  ```

* **Markdown Lint**:

  ```bash
  npx markdownlint-cli2 "**/*.md" --config .markdownlint.json
  ```

* **YAML Lint**:

  ```bash
  yamllint -c .yamllint.yaml .
  ```

## Boundaries and Guardrails

* **NEVER** modify files within the `/obj/` or `/bin/` directories.
* **NEVER** commit secrets, API keys, or sensitive configuration data.
* **ALWAYS** run the Quality Checks before committing changes to ensure code quality standards are met.
* **ASK FIRST** before making significant architectural changes to the core library logic.
