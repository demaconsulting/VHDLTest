# Introduction

This guide describes how to install, configure, and use VHDLTest.

## Purpose

VHDLTest is a .NET tool for running VHDL test benches and generating standard test results files. It simplifies VHDL
unit testing by providing a unified command-line interface for multiple VHDL simulators, generating TRX and JUnit XML
test result files compatible with CI/CD systems, and supporting tool validation for regulated industries.

The workflow below shows how VHDLTest is used: write VHDL test benches and a YAML test configuration, run the
`vhdltest` command locally or in a CI pipeline with an installed simulator, and use the resulting pass/fail results,
TRX or JUnit XML test results files, and exit code as a CI gate and test evidence.

![VHDLTest workflow: write VHDL design files, test benches, and a YAML test configuration, run vhdltest with the config,
simulator, and optional test-name options to compile and simulate each test bench, get pass/fail results, TRX or JUnit XML
test results files, and a pass/fail exit code, then use them as a CI gate, in CI test reports, as test evidence, and in pull
request checks](workflow.svg)

## Scope

This guide covers installation and prerequisites, supported VHDL simulators and their configuration, YAML-based test
configuration, command-line options and usage examples, self-validation for regulated industries, CI/CD integration
for GitHub Actions, Azure DevOps, and Jenkins, and troubleshooting common issues.

The guide requires .NET SDK 10.0 or later and at least one supported VHDL simulator. It applies to Windows, Linux, and
macOS platforms.

## References

- [VHDLTest releases](https://github.com/demaconsulting/VHDLTest/releases) — User Guide, Requirements Document
- [GHDL documentation](https://ghdl.github.io/ghdl/) — Open-source VHDL simulator
- [NVC documentation](https://www.nickg.me.uk/nvc/) — Open-source VHDL simulator and compiler
- [.NET global tools documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) — .NET tool
  installation reference
