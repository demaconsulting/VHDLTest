# VHDLTest

<!-- IMPORTANT: All links in this file must be absolute URLs.
     This file is distributed in packages and relative links will not resolve. -->

[![CI][github-build-badge]](https://github.com/demaconsulting/VHDLTest/actions/workflows/build_on_push.yaml)
[![Quality Gate Status][sonarcloud-quality-badge]][sonarcloud-quality-url]
[![Security Rating][sonarcloud-security-badge]][sonarcloud-security-url]
[![NuGet][nuget-badge]][nuget-url]
![GitHub forks][github-forks-badge]
![GitHub Repo stars][github-stars-badge]
![GitHub contributors][github-contributors-badge]
![GitHub][github-license-badge]

## Overview

VHDLTest is a .NET command-line tool that runs VHDL test benches and generates standard test results files.
It supports multiple VHDL simulators including GHDL, ModelSim, QuestaSim, Active-HDL, NVC, and Vivado Simulator.
The tool is designed for use in regulated industries that require evidence of tool validation.

## Features

- Runs VHDL test benches via a simple YAML configuration file
- Generates standard `.trx` or JUnit XML test results files compatible with CI/CD pipelines
- Supports [GHDL](https://github.com/ghdl/ghdl), [ModelSim](https://eda.sw.siemens.com/en-US/ic/modelsim/),
  [QuestaSim](https://eda.sw.siemens.com/en-US/ic/questa-one/simulation/),
  [Active-HDL](https://www.aldec.com/en/products/fpga_simulation/active-hdl),
  [NVC](https://www.nickg.me.uk/nvc), and
  [Vivado Simulator](https://www.xilinx.com/products/design-tools/vivado.html)
- Configurable simulator paths via environment variables
- Built-in self-validation mode for tool qualification evidence in regulated industries

## Installation

Add VHDLTest to a .NET tool manifest file:

```bash
dotnet new tool-manifest # if you are setting up this repo
dotnet tool install --local DEMAConsulting.VHDLTest
```

The tool can then be executed by:

```bash
dotnet vhdltest <arguments>
```

## Usage

Create a YAML configuration file specifying the VHDL source files and test benches to run:

```yaml
# List of VHDL source files
files:
  - full_adder.vhd
  - full_adder_pass_tb.vhd
  - full_adder_fail_tb.vhd

# List of test benches to execute
tests:
  - full_adder_pass_tb
  - full_adder_fail_tb
```

Run VHDLTest with the configuration file:

```bash
dotnet vhdltest --config test_suite.yaml
```

Generate a test results file for CI environments:

```bash
dotnet vhdltest --config test_suite.yaml --results test_results.trx
```

Full command-line options:

```text
Usage: VHDLTest [options] [tests]

Options:
  -h, -?, --help               Display help
  -v, --version                Display version
  --silent                     Silence console output
  --verbose                    Verbose output
  --validate                   Perform self-validation
  --depth <n>                  Validation report depth (default: 1)
  -l, --log <log.txt>          Log output to file
  -c, --config <config.yaml>   Specify configuration
  -r, --results <out.trx|out.xml>  Specify test results file (.trx or JUnit XML)
  -s, --simulator <name>       Specify simulator
  -0, --exit-0                 Exit with code 0 if test fail
  --                           End of options
```

Before running tests, configure simulator paths via environment variables if needed:

- `VHDLTEST_GHDL_PATH` — path to GHDL folder
- `VHDLTEST_MODELSIM_PATH` — path to ModelSim folder
- `VHDLTEST_QUESTASIM_PATH` — path to QuestaSim folder
- `VHDLTEST_VIVADO_PATH` — path to Vivado folder
- `VHDLTEST_ACTIVEHDL_PATH` — path to ActiveHDL folder
- `VHDLTEST_NVC_PATH` — path to NVC folder

## Building

```pwsh
pwsh ./build.ps1
```

## User Guide

The VHDLTest User Guide is available on the [VHDLTest releases page](https://github.com/demaconsulting/VHDLTest/releases).

## Contributing

We welcome contributions! See [CONTRIBUTING.md](https://github.com/demaconsulting/VHDLTest/blob/main/CONTRIBUTING.md)
for guidelines on submitting pull requests, reporting issues, and contributing to the project.

This project adheres to a [Code of Conduct](https://github.com/demaconsulting/VHDLTest/blob/main/CODE_OF_CONDUCT.md)
to ensure a welcoming environment for all contributors.

## License

This project is licensed under the MIT License —
see [LICENSE](https://github.com/demaconsulting/VHDLTest/blob/main/LICENSE).

## Support

- [Report a bug or request a feature](https://github.com/demaconsulting/VHDLTest/issues)
- [Ask a question or start a discussion](https://github.com/demaconsulting/VHDLTest/discussions)

If you discover a security vulnerability, please review our
[Security Policy](https://github.com/demaconsulting/VHDLTest/blob/main/SECURITY.md) for responsible disclosure.

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
