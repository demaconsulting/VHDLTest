# Introduction

This guide describes how to install, configure, and use VHDLTest.

## Purpose

VHDLTest is a .NET tool for running VHDL test benches and generating standard test results files. It simplifies VHDL
unit testing by providing a unified command-line interface for multiple VHDL simulators, generating TRX test result
files compatible with CI/CD systems, and supporting tool validation for regulated industries.

## Scope

This guide covers installation and prerequisites, supported VHDL simulators and their configuration, YAML-based test
configuration, command-line options and usage examples, self-validation for regulated industries, CI/CD integration
for GitHub Actions, Azure DevOps, and Jenkins, and troubleshooting common issues.

The guide requires .NET SDK 8.0 or later and at least one supported VHDL simulator. It applies to Windows, Linux, and
macOS platforms.

## References

- [VHDLTest releases](https://github.com/demaconsulting/VHDLTest/releases) — User Guide, Requirements Document
- [GHDL documentation](https://ghdl.github.io/ghdl/) — Open-source VHDL simulator
- [NVC documentation](https://www.nickg.me.uk/nvc/) — Open-source VHDL simulator and compiler
- [.NET global tools documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) — .NET tool
  installation reference
