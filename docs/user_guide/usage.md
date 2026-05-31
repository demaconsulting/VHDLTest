# Usage

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

- `-h, -?, --help` - Display help information
- `-v, --version` - Display version information
- `--silent` - Suppress console output
- `--verbose` - Enable verbose output
- `--validate` - Perform self-validation
- `--depth <n>` - Validation report depth (default: 1)
- `-l, --log <log.txt>` - Log output to file
- `-c, --config <config.yaml>` - Specify configuration file
- `-r, --result, --results <out.trx|out.xml>` - Specify test results output file
- `-s, --simulator <name>` - Specify simulator (activehdl, ghdl, mock, modelsim, nvc, questasim, vivado)
- `-0, --exit-0` - Exit with code 0 even if tests fail
- `--` - End of options marker

## Filtering Tests

To run only specific test benches from the configuration file, provide their names as
positional arguments after the options (or after `--` to separate them from option flags):

```bash
dotnet vhdltest --config test_suite.yaml full_adder_pass_tb
```

Multiple test bench names may be specified:

```bash
dotnet vhdltest --config test_suite.yaml -- full_adder_pass_tb full_adder_fail_tb
```

Only the named test benches are executed; any other benches listed in the configuration
file are skipped. When no positional arguments are given, all test benches in the
configuration file are executed.

## Generating Test Results

To generate a TRX test results file for CI/CD integration:

```bash
dotnet vhdltest --config test_suite.yaml --results test_results.trx
```

To generate a JUnit XML test results file instead, use a `.xml` extension:

```bash
dotnet vhdltest --config test_suite.yaml --results test_results.xml
```

The file format is determined by the extension of the results file:

- `.trx` — Microsoft Visual Studio Test Results (TRX) format
- `.xml` — JUnit XML format

The TRX file format is compatible with most CI/CD systems and can be displayed in:

- Azure DevOps
- GitHub Actions
- Jenkins
- TeamCity
- Other systems supporting Visual Studio Test Results format

## Exit Codes

VHDLTest returns the following exit codes:

- `0` - All tests passed (or `--exit-0` was used)
- `Non-zero` — Any error condition, including compilation failure, test failure,
  invalid configuration, or unknown simulator.

## Self-Validation

Self-validation produces a report demonstrating that VHDLTest is functioning correctly. This is useful in regulated
industries where tool validation evidence is required.

### Running Validation

To perform self-validation using the built-in mock simulator:

```bash
dotnet vhdltest --validate --simulator mock
```

Self-validation uses the `mock` simulator to execute embedded VHDL test scenarios without
requiring a real simulator installation. Specifying `--simulator mock` is required to use
the built-in mock; without it, VHDLTest attempts to auto-discover an installed simulator.

### Saving the Validation Report

When `--results` is provided during a `--validate` run, the validation report is written
to the specified file in addition to being displayed on the console. For example:

```bash
dotnet vhdltest --validate --simulator mock --results validation.trx
```

The output format is determined by the file extension (`.trx` for TRX format or `.xml`
for JUnit XML format).

### Validation Report

The validation report contains:

- VHDLTest version
- Machine name
- Operating system version
- DotNet Runtime
- Time Stamp
- Test results

Example validation report:

```text
# DEMAConsulting.VHDLTest

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| VHDLTest Version    | 1.0.0                                              |
| Machine Name        | BUILD-SERVER                                       |
| OS Version          | Microsoft Windows NT 10.0.19045.0                  |
| DotNet Runtime      | .NET 8.0.0                                         |
| Time Stamp          | 2024-01-15 10:30:00 UTC                            |

Tests:

✓ VHDLTest_TestPasses - Passed
✓ VHDLTest_TestFails - Passed

Total Tests: 2
Passed: 2
Failed: 0

Validation Passed
```

### Validation Tests

Each test proves specific functionality works correctly:

- **`VHDLTest_TestPasses`** - The simulator correctly reports passing test benches.
- **`VHDLTest_TestFails`** - The simulator correctly reports failing test benches.

### Validation Failure

On validation failure:

- The tool exits with a non-zero exit code
- The report indicates which validation tests failed
- Error messages provide diagnostic information
