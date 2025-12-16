# VHDLTest Tool

![GitHub forks][github-forks-badge]
![GitHub Repo stars][github-stars-badge]
![GitHub contributors][github-contributors-badge]
![GitHub][github-license-badge]
![Build][github-build-badge]
[![Quality Gate Status][sonarcloud-quality-badge]][sonarcloud-quality-url]
[![Security Rating][sonarcloud-security-badge]][sonarcloud-security-url]
[![NuGet][nuget-badge]][nuget-url]

This tool runs VHDL test benches and generates standard test results files.

## Installation

The following will add VHDLTest to a Dotnet tool manifest file:

```bash
dotnet new tool-manifest # if you are setting up this repo
dotnet tool install --local DEMAConsulting.VHDLTest
```

The tool can then be executed by:

```bash
dotnet vhdltest <arguments>
```

## Options

```text
Usage: VHDLTest [options] [tests]

Options:
  -h, --help                   Display help
  -v, --version                Display version
  --silent                     Silence console output
  --verbose                    Verbose output
  --validate                   Perform self-validation
  -c, --config <config.yaml>   Specify configuration
  -r, --results <out.trx>      Specify test results file
  -s, --simulator <name>       Specify simulator
  -0, --exit-0                 Exit with code 0 if test fail
  --                           End of options
```

## Supported Simulators

The current list of supported simulators are:

* [GHDL][ghdl-url]
* [ModelSim][modelsim-url]
* [Vivado][vivado-url]
* [ActiveHDL][activehdl-url]
* [NVC][nvc-url]

## Configuration

VHDLTest needs a YAML configuration file specifying the VHDL files and test benches.

```yaml
# List of VHDL source files
files:
 - full_adder.vhd
 - full_adder_pass_tb.vhd
 - full_adder_fail_tb.vhd
 - half_adder.vhd
 - half_adder_pass_tb.vhd
 - half_adder_fail_tb.vhd

# List of test benches to execute
tests:
 - full_adder_pass_tb
 - full_adder_fail_tb
 - half_adder_pass_tb
 - half_adder_fail_tb
```

## Running Tests

Before running the tests, it may be necessary to configure where the simulators are installed.
This can be done through environment variables:

* VHDLTEST_GHDL_PATH = path to GHDL folder
* VHDLTEST_MODELSIM_PATH = path to ModelSim folder
* VHDLTEST_VIVADO_PATH = path to Vivado folder
* VHDLTEST_ACTIVEHDL_PATH = path to ActiveHDL folder
* VHDLTEST_NVC_PATH = path to NVC folder

To run the tests, execute VHDLTest with the name of the configuration file.

```bash
dotnet VHDLTest --config test_suite.yaml
```

A test results file can be generated when working in CI environments.

```bash
dotnet VHDLTest --config test_suite.yaml --results test_results.trx
```

## Self Validation

Running self-validation produces a report containing the following information:

```text
# DEMAConsulting.VHDLTest

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| VHDLTest Version    | <version>                                          |
| Machine Name        | <machine-name>                                     |
| OS Version          | <os-version>                                       |
| DotNet Runtime      | <dotnet-runtime-version>                           |
| Time Stamp          | <timestamp>                                        |

Tests:

- TestPasses: Passed
- TestFails: Passed

Validation Passed
```

On validation failure the tool will exit with a non-zero exit code.

This report may be useful in regulated industries requiring evidence of tool validation.

## Code Quality

This project maintains high code quality standards through:

* **Static Analysis**: Microsoft.CodeAnalysis.NetAnalyzers provides comprehensive code analysis
* **Code Coverage**: Coverlet tracks test coverage during builds
* **SonarCloud**: Continuous code quality and security analysis
* **EditorConfig**: Consistent code formatting across all editors
* **Spell Checking**: cspell configuration for documentation quality
* **Markdown Linting**: markdownlint for consistent documentation formatting
* **Automated Checks**: Quality checks workflow with spell checking and markdown linting
* **Dependency Management**: Dependabot for weekly dependency updates

### For Contributors

The project includes:

* `.editorconfig` - Formatting rules for all editors
* `.cspell.json` - Spell checking configuration
* `.markdownlint.json` - Markdown linting rules

Build with `dotnet build` to see code quality feedback.

## Contributing

We welcome contributions! Please see our [Contributing Guidelines][contributing-url] for details on how to submit
pull requests, report issues, and contribute to the project.

## Code of Conduct

This project adheres to a [Code of Conduct][code-of-conduct-url] to ensure a welcoming environment for all contributors.

## Security

Security is a top priority for this project. If you discover a security vulnerability, please review our
[Security Policy][security-url] for information on how to report it responsibly.

<!-- Link References -->
[nuget-badge]: https://img.shields.io/nuget/v/DemaConsulting.VHDLTest?style=plastic
[nuget-url]: https://www.nuget.org/packages/DemaConsulting.VHDLTest
[github-forks-badge]: https://img.shields.io/github/forks/demaconsulting/VHDLTest?style=plastic
[github-stars-badge]: https://img.shields.io/github/stars/demaconsulting/VHDLTest?style=plastic
[github-contributors-badge]: https://img.shields.io/github/contributors/demaconsulting/VHDLTest?style=plastic
[github-license-badge]: https://img.shields.io/github/license/demaconsulting/VHDLTest?style=plastic
[github-build-badge]: https://github.com/demaconsulting/VHDLTest/actions/workflows/build_on_push.yaml/badge.svg
[sonarcloud-quality-badge]: https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_VHDLTest&metric=alert_status
[sonarcloud-quality-url]: https://sonarcloud.io/summary/new_code?id=demaconsulting_VHDLTest
[sonarcloud-security-badge]: https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_VHDLTest&metric=security_rating
[sonarcloud-security-url]: https://sonarcloud.io/summary/new_code?id=demaconsulting_VHDLTest
[ghdl-url]: https://github.com/ghdl/ghdl
[modelsim-url]: https://eda.sw.siemens.com/en-US/ic/modelsim/
[vivado-url]: https://www.xilinx.com/products/design-tools/vivado.html
[activehdl-url]: https://www.aldec.com/en/products/fpga_simulation/active-hdl
[nvc-url]: https://www.nickg.me.uk/nvc
[contributing-url]: https://github.com/demaconsulting/VHDLTest/blob/main/CONTRIBUTING.md
[code-of-conduct-url]: https://github.com/demaconsulting/VHDLTest/blob/main/CODE_OF_CONDUCT.md
[security-url]: https://github.com/demaconsulting/VHDLTest/blob/main/SECURITY.md
