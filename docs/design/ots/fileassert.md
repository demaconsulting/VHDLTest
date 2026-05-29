## DemaConsulting.FileAssert Integration Design

### Purpose

DemaConsulting.FileAssert performs normalized comparison of generated output files against
reference baselines in the CI pipeline. It prevents unintentional changes to generated
documentation and traceability reports from silently passing the build. FileAssert is not
deployed with VHDLTest.

### Features Used

- **File comparison**: compares a generated output file against a committed baseline file
  and exits non-zero if they differ, failing the build.

### Integration Pattern

FileAssert is available as a local dotnet tool (`.config/dotnet-tools.json`) and is
invoked in CI pipeline steps or PowerShell scripts to verify that generated artifacts
(such as requirements reports or traceability matrices) match their expected committed
baselines. Invocation takes the form:

```bash
dotnet fileassert expected-file.md generated-file.md
```

A non-zero exit code from FileAssert causes the CI step to fail, preventing outputs that
diverge from the baseline from reaching the release artifacts.
