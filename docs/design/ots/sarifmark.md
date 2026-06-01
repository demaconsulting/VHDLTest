## DemaConsulting.SarifMark Integration Design

### Purpose

DemaConsulting.SarifMark converts SARIF (Static Analysis Results Interchange Format)
output from CodeQL code scanning into a human-readable markdown report. The report is
included in the release code quality artifacts. SarifMark is not deployed with VHDLTest.

### Features Used

- **SARIF reading**: parses the CodeQL SARIF file produced by the `codeql` CI job.
- **Markdown report generation**: renders findings as a structured markdown document.
- **Self-validation**: `sarifmark --validate --results ...` executes SarifMark's own
  internal test suite and writes TRX results.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, SarifMark reads the SARIF
output that was uploaded by the `codeql` job and produces a markdown quality report:

```bash
dotnet sarifmark \
  --sarif artifacts/csharp.sarif \
  --report docs/code_quality/codeql-quality.md \
  --heading "VHDLTest CodeQL Analysis" \
  --report-depth 1
```

The generated markdown is then converted to HTML by PandocTool and to PDF by WeasyprintTool
as part of the code quality document.
