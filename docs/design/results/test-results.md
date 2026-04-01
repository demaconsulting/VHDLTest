# TestResults Unit Design

## Overview

`TestResults.cs` is a collection of `TestResult` instances with serialization
support for TRX and JUnit XML formats.

## Responsibilities

- Hold a list of `TestResult` instances
- Serialize results to TRX format for MSTest-compatible reporting
- Serialize results to JUnit XML format for CI/CD system integration
- Compute aggregate pass/fail counts
