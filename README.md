# VHDLTest

This tool runs VHDL test benches and generates standard test results files.


# Options

```
Usage: VHDLTest [options] [tests]

Options:
  -h|-?|--help                 Display help
  -v|--version                 Display version
  -c|--config <config.yaml>    Specify configuration
    |--verbose                 Verbose output
  -r|--results <out.trx>       Specify test results file
  -s|--simulator <name>        Specify simulator
  -0|--exit-0                  Exit with code 0 if test fail
  --                           End of options
```


# Supported Simulators

The current list of supported simulators are:

* [GHDL](https://github.com/ghdl/ghdl)
* [ModelSim](https://eda.sw.siemens.com/en-US/ic/modelsim/)
* [Vivado](https://www.xilinx.com/products/design-tools/vivado.html)
* [ActiveHDL](https://www.aldec.com/en/products/fpga_simulation/active-hdl)


# Installing

VHDLTest is distributed as a dotnet tool through NuGet.org and can be installed globally:

```
dotnet tool install -g DEMAConsulting.VHDLTest

VHDLTest --help
```


Additionally it can be installed as a local tool in the working folder:

```
dotnet tool install DEMAConsulting.VHDLTest

dotnet VHDLTest --help
```


# Configuration

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


# Running Tests

Before running the tests, it may be necessary to configure where the simulators are installed.
This can be done through environment variables:
* VHDLTEST_GHDL_PATH = path to GHDL folder
* VHDLTEST_MODELSIM_PATH = path to ModelSim folder
* VHDLTEST_VIVADO_PATH = path to Vivado folder
* VHDLTEST_ACTIVEHDL_PATH = path to ActiveHDL folder


To run the tests, execute VHDLTest with the name of the configuration file.

```
dotnet VHDLTest --config test_suite.yaml
```

A test results file can be generated when working in CI environments.

```
dotnet VHDLTest --config test_suite.yaml --results test_results.trx
```
