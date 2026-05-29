# Installation

## Prerequisites

Before installing VHDLTest, ensure you have:

- **.NET SDK**: Version 8.0 or later
- **VHDL Simulator**: At least one of the supported simulators installed

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
