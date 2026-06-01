# Introduction

This document provides the requirements Trace Matrix for VHDLTest, mapping each requirement to its corresponding
test evidence.

## Purpose

To demonstrate that every requirement is covered by at least one passing test, providing compliance evidence for
VHDLTest.

The matrix aggregates test results from unit tests and integration tests across multiple .NET runtime versions and
platforms, and maps them to the requirements defined in the VHDLTest Requirements Document. Requirements are
considered verified when all associated test cases have been executed and passed with adequate coverage.

## Scope

This document covers all requirements defined in `docs/reqstream/` for VHDLTest and their test evidence, drawn from:

- Unit tests executed on .NET runtime versions net8.0, net9.0, and net10.0
- GHDL integration tests on multiple platforms and .NET versions
- NVC integration tests on multiple platforms and .NET versions

The matrix identifies requirements with full coverage, partial coverage, or no coverage, and highlights failed
tests requiring remediation.

## References

- [VHDLTest releases](https://github.com/demaconsulting/VHDLTest/releases) — Requirements Document
