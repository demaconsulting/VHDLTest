# VHDLTest Requirements

## Command-Line Interface

| ID | Title |
| :- | :---- |
| CLI-001 | VHDLTest shall display usage information when run without arguments. |
| CLI-002 | VHDLTest shall display version information when requested. |
| CLI-003 | VHDLTest shall display help information when requested. |

## Test Execution

| ID | Title |
| :- | :---- |
| TEST-001 | VHDLTest shall execute VHDL test benches and return zero exit code on success. |
| TEST-002 | VHDLTest shall return non-zero exit code when compilation fails. |
| TEST-003 | VHDLTest shall return non-zero exit code when tests fail. |
| TEST-004 | VHDLTest shall support exit-0 mode to always return zero exit code. |

## Validation

| ID | Title |
| :- | :---- |
| VAL-001 | VHDLTest shall support self-validation mode. |
| VAL-002 | VHDLTest shall support configurable validation report depth. |
| VAL-003 | VHDLTest shall save validation results to file. |
| VAL-004 | VHDLTest shall include system information in validation reports. |

## Simulator Support

| ID | Title |
| :- | :---- |
| SIM-001 | VHDLTest shall support the GHDL simulator. |
| SIM-002 | VHDLTest shall support the ModelSim simulator. |
| SIM-003 | VHDLTest shall support the QuestaSim simulator. |
| SIM-004 | VHDLTest shall support the Vivado simulator. |
| SIM-005 | VHDLTest shall support the ActiveHDL simulator. |
| SIM-006 | VHDLTest shall support the NVC simulator. |

## Platform Support

| ID | Title |
| :- | :---- |
| PLT-001 | VHDLTest shall run on Linux operating systems. |
| PLT-002 | VHDLTest shall run on Windows operating systems. |
| PLT-003 | VHDLTest shall support .NET 8.0 runtime. |
| PLT-004 | VHDLTest shall support .NET 9.0 runtime. |
| PLT-005 | VHDLTest shall support .NET 10.0 runtime. |

