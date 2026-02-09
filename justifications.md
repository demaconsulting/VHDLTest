# VHDLTest Requirements

## Command-Line Interface

### CLI-001

**VHDLTest shall display usage information when run without arguments.**

Provides immediate guidance to users who are unfamiliar with the tool or have
forgotten the command syntax. This improves user experience by making the tool
self-documenting and reduces the need for external documentation lookups.


### CLI-002

**VHDLTest shall display version information when requested.**

Enables users to verify which version of VHDLTest is installed, which is critical
for troubleshooting, compatibility checks, and ensuring reproducible builds. This
is a standard practice for CLI tools and supports issue reporting.


### CLI-003

**VHDLTest shall display help information when requested.**

Provides comprehensive command-line documentation directly within the tool,
allowing users to quickly reference available options and syntax without
consulting external documentation. Supports multiple help flags for user convenience.


## Test Execution

### TEST-001

**VHDLTest shall execute VHDL test benches and return zero exit code on success.**

Enables integration with CI/CD pipelines and build systems that rely on exit
codes to determine test success or failure. A zero exit code is the standard
convention for successful command execution in Unix-like systems.


### TEST-002

**VHDLTest shall return non-zero exit code when compilation fails.**

Allows CI/CD systems and automated workflows to detect compilation errors and
halt the build process appropriately. This prevents invalid or incomplete designs
from progressing through the development pipeline.


### TEST-003

**VHDLTest shall return non-zero exit code when tests fail.**

Enables automated test environments to detect test failures and take appropriate
action (e.g., failing the build, sending notifications). This is essential for
continuous integration and maintaining code quality.


### TEST-004

**VHDLTest shall support exit-0 mode to always return zero exit code.**

Provides flexibility for scenarios where test results need to be collected and
analyzed without failing the build process. Useful for generating test reports
in pipelines where failures are handled by separate post-processing steps.


## Validation

### VAL-001

**VHDLTest shall support self-validation mode.**

Enables users to verify that VHDLTest is correctly installed and configured in
their environment. This diagnostic capability helps identify configuration issues
before running actual test suites, reducing troubleshooting time.


### VAL-002

**VHDLTest shall support configurable validation report depth.**

Allows users to control the level of detail in validation reports based on their
needs. Shallow reports provide quick checks while deeper reports offer comprehensive
diagnostic information for complex troubleshooting scenarios.


### VAL-003

**VHDLTest shall save validation results to file.**

Enables persistent storage of validation results for documentation, compliance,
and historical comparison purposes. Saved reports can be shared with team members
or included in bug reports for support.


### VAL-004

**VHDLTest shall include system information in validation reports.**

Provides essential context for validation results by capturing the execution
environment (OS version, runtime, etc.). This information is crucial for
reproducing issues and diagnosing environment-specific problems.


## Simulator Support

### SIM-001

**VHDLTest shall support the GHDL simulator.**

GHDL is the most widely-used open-source VHDL simulator and is essential for
teams working with free and open-source toolchains. Supporting GHDL enables
cost-effective VHDL development and testing without commercial licenses.


### SIM-002

**VHDLTest shall support the ModelSim simulator.**

ModelSim is an industry-standard commercial VHDL simulator widely used in
professional FPGA and ASIC development. Support for ModelSim is critical for
enterprise users and teams with existing ModelSim-based workflows.


### SIM-003

**VHDLTest shall support the QuestaSim simulator.**

QuestaSim is the advanced verification tool from Siemens/Mentor Graphics, offering
enhanced debugging and coverage features. Supporting QuestaSim enables VHDLTest
to integrate with high-end verification environments.


### SIM-004

**VHDLTest shall support the Vivado simulator.**

Vivado is Xilinx's primary design suite and simulator for their FPGA products.
Supporting Vivado simulator is essential for teams developing designs targeting
Xilinx/AMD FPGAs and enables seamless integration with the Vivado toolchain.


### SIM-005

**VHDLTest shall support the ActiveHDL simulator.**

Active-HDL from Aldec is a popular VHDL simulator particularly in the aerospace
and defense industries. Supporting Active-HDL ensures VHDLTest can be used in
regulated environments and specialized application domains.


### SIM-006

**VHDLTest shall support the NVC simulator.**

NVC is a modern, fast open-source VHDL simulator with good standards compliance.
Supporting NVC provides users with an alternative open-source option that offers
better performance than GHDL in certain scenarios and supports newer VHDL standards.


## Platform Support

### PLT-001

**VHDLTest shall run on Linux operating systems.**

Linux is the dominant platform for FPGA/ASIC development and continuous integration
environments. Supporting Linux ensures VHDLTest can be used in CI/CD pipelines and
on the primary development platform for embedded systems and hardware design.


### PLT-002

**VHDLTest shall run on Windows operating systems.**

Windows is widely used in corporate environments and for desktop-based FPGA
development. Supporting Windows ensures VHDLTest is accessible to the broadest
possible user base including enterprise users with Windows-standardized environments.


### PLT-003

**VHDLTest shall support .NET 8.0 runtime.**

.NET 8.0 is a Long-Term Support (LTS) release supported until November 2026.
Supporting this LTS version ensures stability and long-term usability for users
who require extended support lifecycles in production environments.


### PLT-004

**VHDLTest shall support .NET 9.0 runtime.**

.NET 9.0 is a Standard Term Support (STS) release that provides access to the
latest .NET features and performance improvements. Supporting .NET 9.0 enables
users to leverage modern runtime capabilities while it remains in support.


### PLT-005

**VHDLTest shall support .NET 10.0 runtime.**

.NET 10.0 is the upcoming Long-Term Support (LTS) release scheduled for November
2025. Supporting .NET 10.0 ensures VHDLTest remains compatible with the latest
long-term supported runtime and benefits from future performance and security updates.


