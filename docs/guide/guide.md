# Introduction

VHDLTest is a .NET tool for running VHDL test benches and generating standard test results files. This guide provides comprehensive documentation for installing, configuring, and using VHDLTest in your VHDL development workflow.

## Purpose

VHDLTest simplifies the process of running VHDL unit tests by:

* Providing a unified interface for multiple VHDL simulators
* Generating standard test result files compatible with CI/CD systems
* Supporting automated testing workflows
* Enabling tool validation for regulated industries

## Key Features

* **Multi-Simulator Support**: Works with GHDL, ModelSim, Vivado, ActiveHDL, and NVC
* **Standard Test Results**: Generates TRX (Visual Studio Test Results) files
* **YAML Configuration**: Simple, readable test configuration
* **CI/CD Integration**: Designed for automated testing pipelines
* **Self-Validation**: Built-in validation for tool qualification

# Installation

## Prerequisites

Before installing VHDLTest, ensure you have:

* **.NET SDK**: Version 8.0 or later
* **VHDL Simulator**: At least one of the supported simulators installed

## Installation Methods

### Local Installation

To add VHDLTest to a .NET tool manifest file:

```bash
dotnet new tool-manifest # if you are setting up this repo
dotnet tool install --local DEMAConsulting.VHDLTest
```

The tool can then be executed by:

```bash
dotnet vhdltest <arguments>
```

### Global Installation

For global installation across all projects:

```bash
dotnet tool install --global DEMAConsulting.VHDLTest
```

Then execute directly:

```bash
vhdltest <arguments>
```

## Verifying Installation

To verify VHDLTest is installed correctly:

```bash
dotnet vhdltest --version
```

This will display the installed version number.

# Supported Simulators

VHDLTest supports the following VHDL simulators:

## GHDL

[GHDL](https://github.com/ghdl/ghdl) is an open-source VHDL simulator.

**Configuration**: Set the `VHDLTEST_GHDL_PATH` environment variable to the GHDL installation folder if not in PATH.

## ModelSim

[ModelSim](https://eda.sw.siemens.com/en-US/ic/modelsim/) is a commercial HDL simulator from Siemens.

**Configuration**: Set the `VHDLTEST_MODELSIM_PATH` environment variable to the ModelSim installation folder.

## Vivado

[Vivado](https://www.xilinx.com/products/design-tools/vivado.html) is Xilinx's design suite for FPGAs.

**Configuration**: Set the `VHDLTEST_VIVADO_PATH` environment variable to the Vivado installation folder.

## ActiveHDL

[ActiveHDL](https://www.aldec.com/en/products/fpga_simulation/active-hdl) is a commercial HDL simulator from Aldec.

**Configuration**: Set the `VHDLTEST_ACTIVEHDL_PATH` environment variable to the ActiveHDL installation folder.

## NVC

[NVC](https://www.nickg.me.uk/nvc) is an open-source VHDL simulator and compiler.

**Configuration**: Set the `VHDLTEST_NVC_PATH` environment variable to the NVC installation folder if not in PATH.

# Configuration

VHDLTest uses YAML configuration files to specify VHDL source files and test benches.

## Configuration File Format

A basic configuration file (`test_suite.yaml`) contains:

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

## Files Section

The `files` section lists all VHDL source files in dependency order:

* List dependencies before files that use them
* Include both design files and test bench files
* Use relative paths from the configuration file location

## Tests Section

The `tests` section specifies which test benches to execute:

* List the entity name of each test bench
* Tests are executed in the order listed
* Test bench entities must be defined in the files section

## Environment Variables

Configure simulator paths using environment variables:

* `VHDLTEST_GHDL_PATH` - Path to GHDL installation
* `VHDLTEST_MODELSIM_PATH` - Path to ModelSim installation
* `VHDLTEST_VIVADO_PATH` - Path to Vivado installation
* `VHDLTEST_ACTIVEHDL_PATH` - Path to ActiveHDL installation
* `VHDLTEST_NVC_PATH` - Path to NVC installation

These are only required if simulators are not in the system PATH.

# Running Tests

## Basic Usage

To run tests with a configuration file:

```bash
dotnet vhdltest --config test_suite.yaml
```

Or with a specific simulator:

```bash
dotnet vhdltest --config test_suite.yaml --simulator ghdl
```

## Command Line Options

VHDLTest supports the following command line options:

* `-h, --help` - Display help information
* `-v, --version` - Display version information
* `--silent` - Suppress console output
* `--verbose` - Enable verbose output
* `--validate` - Perform self-validation
* `-c, --config <config.yaml>` - Specify configuration file
* `-r, --results <out.trx>` - Specify test results output file
* `-s, --simulator <name>` - Specify simulator (ghdl, modelsim, vivado, activehdl, nvc)
* `-0, --exit-0` - Exit with code 0 even if tests fail
* `--` - End of options marker

## Generating Test Results

To generate a TRX test results file for CI/CD integration:

```bash
dotnet vhdltest --config test_suite.yaml --results test_results.trx
```

The TRX file format is compatible with most CI/CD systems and can be displayed in:

* Azure DevOps
* GitHub Actions
* Jenkins
* TeamCity
* Other systems supporting Visual Studio Test Results format

## Exit Codes

VHDLTest returns the following exit codes:

* `0` - All tests passed (or `--exit-0` was used)
* `Non-zero` - One or more tests failed

# Self-Validation

## Purpose

Self-validation produces a report demonstrating that VHDLTest is functioning correctly. This is useful in regulated industries where tool validation evidence is required.

## Running Validation

To perform self-validation:

```bash
dotnet vhdltest --validate --simulator ghdl
```

## Validation Report

The validation report contains:

* VHDLTest version
* Machine name
* Operating system version
* .NET runtime version
* Timestamp
* Test results

Example validation report:

```text
# DEMAConsulting.VHDLTest

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| VHDLTest Version    | 1.0.0                                              |
| Machine Name        | BUILD-SERVER                                       |
| OS Version          | Microsoft Windows NT 10.0.19045.0                  |
| DotNet Runtime      | .NET 8.0.0                                         |
| Time Stamp          | 2024-01-15T10:30:00Z                               |

Tests:

- TestPasses: Passed
- TestFails: Passed

Validation Passed
```

## Validation Failure

On validation failure:

* The tool exits with a non-zero exit code
* The report indicates which validation tests failed
* Error messages provide diagnostic information

# CI/CD Integration

## GitHub Actions

Example GitHub Actions workflow:

```yaml
name: VHDL Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'
      
      - name: Setup GHDL
        uses: ghdl/setup-ghdl@v1
        with:
          version: nightly
      
      - name: Install VHDLTest
        run: dotnet tool install --global DEMAConsulting.VHDLTest
      
      - name: Run Tests
        run: vhdltest --config test_suite.yaml --results results.trx
      
      - name: Publish Test Results
        uses: dorny/test-reporter@v1
        if: always()
        with:
          name: VHDL Tests
          path: results.trx
          reporter: dotnet-trx
```

## Azure DevOps

Example Azure DevOps pipeline:

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.x'
  
  - script: |
      dotnet tool install --global DEMAConsulting.VHDLTest
    displayName: 'Install VHDLTest'
  
  - script: |
      vhdltest --config test_suite.yaml --results $(Build.ArtifactStagingDirectory)/results.trx
    displayName: 'Run VHDL Tests'
  
  - task: PublishTestResults@2
    inputs:
      testResultsFormat: 'VSTest'
      testResultsFiles: '**/results.trx'
```

## Jenkins

Example Jenkinsfile:

```groovy
pipeline {
    agent any
    
    stages {
        stage('Setup') {
            steps {
                sh 'dotnet tool install --global DEMAConsulting.VHDLTest'
            }
        }
        
        stage('Test') {
            steps {
                sh 'vhdltest --config test_suite.yaml --results results.trx'
            }
        }
        
        stage('Publish Results') {
            steps {
                mstest testResultsFile: 'results.trx'
            }
        }
    }
}
```

# Troubleshooting

## Common Issues

### Simulator Not Found

**Problem**: VHDLTest cannot find the VHDL simulator.

**Solution**: 
* Ensure the simulator is installed
* Set the appropriate environment variable (e.g., `VHDLTEST_GHDL_PATH`)
* Add the simulator to your system PATH

### Compilation Errors

**Problem**: VHDL files fail to compile.

**Solution**:
* Verify VHDL syntax is correct
* Check that files are listed in dependency order
* Ensure all required libraries are included

### Tests Not Executing

**Problem**: Test benches are not running.

**Solution**:
* Verify test bench entity names match the `tests` section
* Ensure test bench files are included in the `files` section
* Check that test benches have correct structure

### Permission Errors

**Problem**: Cannot write test results file.

**Solution**:
* Ensure the output directory exists
* Verify write permissions on the output directory
* Check disk space availability

## Debug Mode

Enable verbose output for troubleshooting:

```bash
dotnet vhdltest --config test_suite.yaml --verbose
```

This provides detailed information about:

* File processing
* Compilation steps
* Test execution
* Simulator output

# Best Practices

## Test Organization

* **Separate Test Files**: Keep test benches in separate files from design units
* **Naming Convention**: Use `_tb` suffix for test bench files (e.g., `adder_tb.vhd`)
* **Dependency Order**: List files in dependency order in configuration

## Configuration Management

* **Version Control**: Keep configuration files in version control
* **Multiple Configs**: Use different configuration files for different test suites
* **Documentation**: Comment configuration files to explain purpose

## CI/CD Integration

* **Automated Testing**: Run VHDLTest on every commit
* **Test Results**: Always generate and publish test results
* **Multiple Simulators**: Test with multiple simulators when possible
* **Validation**: Include self-validation in release pipelines

## Test Design

* **Assertions**: Use VHDL assertions to verify behavior
* **Coverage**: Aim for comprehensive test coverage
* **Independence**: Ensure tests are independent and can run in any order
* **Clear Output**: Provide clear pass/fail indicators

# Appendix

## Version History

See the [GitHub releases page](https://github.com/demaconsulting/VHDLTest/releases) for detailed version history.

## License

VHDLTest is licensed under the MIT License. See the [LICENSE](https://github.com/demaconsulting/VHDLTest/blob/main/LICENSE) file for details.

## Contributing

Contributions are welcome! Please see the [Contributing Guidelines](https://github.com/demaconsulting/VHDLTest/blob/main/CONTRIBUTING.md) for details.

## Support

For issues, questions, or feature requests:

* **GitHub Issues**: <https://github.com/demaconsulting/VHDLTest/issues>
* **Documentation**: <https://github.com/demaconsulting/VHDLTest>

## Additional Resources

* **VHDL Standards**: IEEE Std 1076-2019
* **GHDL Documentation**: <https://ghdl.github.io/ghdl/>
* **NVC Documentation**: <https://www.nickg.me.uk/nvc/>
* **.NET Tool Documentation**: <https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools>
