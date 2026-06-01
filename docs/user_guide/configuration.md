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

- List dependencies before files that use them
- Include both design files and test bench files
- Use relative paths from the configuration file location

## Tests Section

The `tests` section specifies which test benches to execute:

- List the entity name of each test bench
- Tests are executed in the order listed
- Test bench entities must be defined in the files section

## Environment Variables

Configure simulator paths using environment variables:

- `VHDLTEST_ACTIVEHDL_PATH` - Path to Active-HDL installation
- `VHDLTEST_GHDL_PATH` - Path to GHDL installation
- `VHDLTEST_MODELSIM_PATH` - Path to ModelSim installation
- `VHDLTEST_NVC_PATH` - Path to NVC installation
- `VHDLTEST_QUESTASIM_PATH` - Path to QuestaSim installation
- `VHDLTEST_VIVADO_PATH` - Path to Vivado installation

These are only required if simulators are not in the system PATH.
