# VHDLTest Architecture

This document provides an overview of the VHDLTest tool architecture, design decisions, and key components.

## Overview

VHDLTest is a command-line tool written in C# that executes VHDL test benches using various HDL simulators and generates standardized test reports. The tool follows a modular architecture with clear separation of concerns.

## Design Goals

1. **Simulator Independence**: Support multiple VHDL simulators through a common interface
2. **Standards Compliance**: Generate standard test result formats (TRX)
3. **Cross-Platform**: Run on Windows, Linux, and macOS
4. **Tool Validation**: Support self-validation for regulated industries
5. **Minimal Dependencies**: Keep external dependencies to a minimum

## Architecture Layers

### 1. Entry Point (`Program.cs`)

The main entry point handles:
- Command-line argument parsing
- Exception handling and error reporting
- Version information display
- Delegating to the appropriate execution path

### 2. Context (`Context.cs`)

The Context class manages:
- Parsed command-line arguments
- Output configuration (silent, verbose, logging)
- Exit code management
- Console output formatting

### 3. Configuration (`ConfigDocument.cs`, `Options.cs`)

Configuration management includes:
- YAML configuration file parsing using YamlDotNet
- Working directory resolution
- File and test bench enumeration

Configuration file format:
```yaml
files:
  - source1.vhd
  - source2.vhd

tests:
  - testbench1
  - testbench2
```

### 4. Simulator Abstraction (`Simulators/`)

The simulator layer provides a common interface for HDL simulators:

#### Base Classes
- **`Simulator`**: Abstract base class defining the compile/test interface
- **`SimulatorFactory`**: Factory for creating simulator instances

#### Concrete Simulators
- **`GhdlSimulator`**: GHDL (open-source VHDL simulator)
- **`ModelSimSimulator`**: ModelSim (Mentor Graphics)
- **`VivadoSimulator`**: Vivado Simulator (Xilinx)
- **`ActiveHdlSimulator`**: Active-HDL (Aldec)
- **`NvcSimulator`**: NVC (open-source VHDL simulator)
- **`MockSimulator`**: Mock for testing purposes

Each simulator implementation:
1. Generates appropriate compilation scripts
2. Executes the simulator
3. Parses simulator output
4. Reports success/failure

### 5. Test Execution (`Run/`)

The test execution layer processes simulator output:

#### Components
- **`RunProgram`**: Executes external processes (simulators)
- **`RunProcessor`**: Parses simulator output using configurable rules
- **`RunLineRule`**: Defines patterns for parsing output lines
- **`RunLine`**: Represents a classified output line
- **`RunResults`**: Aggregates execution results

#### Output Classification
Lines are classified as:
- **Info**: General information
- **Report**: Test status messages
- **Warning**: Non-fatal issues
- **Error**: Errors and failures
- **Fatal**: Fatal errors

### 6. Results Management (`Results/`)

Test results are managed and reported through:

- **`TestResult`**: Individual test result with status, duration, and messages
- **`TestResults`**: Collection of test results with:
  - Summary statistics (passed, failed, skipped)
  - TRX file generation for CI/CD integration
  - Console output formatting

### 7. Validation (`Validation.cs`)

Self-validation capabilities for regulated industries:
- Embedded test resources
- Version information reporting
- Environment information capture
- Validation report generation

## Key Design Patterns

### Factory Pattern
`SimulatorFactory` creates simulator instances based on name or environment detection.

### Strategy Pattern
Each simulator implementation provides a different strategy for compilation and testing.

### Template Method Pattern
`Simulator` base class defines the overall workflow while concrete implementations provide specific steps.

### Builder Pattern
Simulators build command scripts dynamically based on configuration.

## Data Flow

1. **Initialization**
   - Parse command-line arguments → `Context`
   - Load configuration file → `ConfigDocument`
   - Create simulator instance → `SimulatorFactory`

2. **Compilation**
   - Generate compilation script → Simulator
   - Execute compilation → `RunProgram`
   - Parse output → `RunProcessor`
   - Check for errors → `RunResults`

3. **Testing**
   - For each test bench:
     - Generate test script → Simulator
     - Execute test → `RunProgram`
     - Parse output → `RunProcessor`
     - Create test result → `TestResult`

4. **Reporting**
   - Aggregate results → `TestResults`
   - Generate console summary
   - Optionally save TRX file
   - Return appropriate exit code

## Environment Variables

Simulators are located using environment variables:
- `VHDLTEST_GHDL_PATH`: Path to GHDL installation
- `VHDLTEST_MODELSIM_PATH`: Path to ModelSim installation
- `VHDLTEST_VIVADO_PATH`: Path to Vivado installation
- `VHDLTEST_ACTIVEHDL_PATH`: Path to Active-HDL installation
- `VHDLTEST_NVC_PATH`: Path to NVC installation

## Extension Points

### Adding a New Simulator

1. Create a new class inheriting from `Simulator`
2. Implement `Compile(Context, Options)` method
3. Implement `Test(Context, Options, string)` method
4. Add detection logic to `SimulatorFactory`
5. Add corresponding environment variable support

### Adding New Output Parsing Rules

Extend `RunLineRule` with new patterns for simulator-specific output formats.

## Testing Strategy

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test simulator interfaces with mock implementations
- **Validation Tests**: Embedded validation test benches

## Dependencies

- **.NET 8/9/10**: Target frameworks
- **YamlDotNet**: YAML parsing
- **MSTest**: Testing framework
- **Coverlet**: Code coverage

## Build and Deployment

The tool is distributed as a .NET tool package:
- NuGet package: `DemaConsulting.VHDLTest`
- Command name: `vhdltest`
- Cross-platform support via .NET runtime

## Future Considerations

- Additional simulator support (e.g., QuestaSim)
- JSON configuration format support
- Parallel test execution
- Test result caching
- Enhanced error diagnostics
- Plugin architecture for custom simulators
