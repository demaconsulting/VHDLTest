## DemaConsulting.SarifMark Verification

### Verification Approach

DemaConsulting.SarifMark is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes SarifMark to convert the CodeQL SARIF output
into a markdown report. A passing CI step constitutes evidence that SarifMark correctly
read the SARIF input and rendered the markdown report. SarifMark also provides an explicit
`--validate` self-validation mode that writes TRX test results consumed by ReqStream.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (`windows-latest`, build-docs job). The CodeQL
SARIF artifact (`artifacts/csharp.sarif`) produced by the `codeql` job must be available
before the SarifMark step runs.

### Acceptance Criteria

The SarifMark step in the `build-docs` CI job completes with exit code 0. A passing step
constitutes evidence that SarifMark correctly read the CodeQL SARIF file and rendered
the markdown code quality report.

### Test Scenarios

- **SARIF reading**: `dotnet sarifmark --sarif artifacts/csharp.sarif ...` reads the
  CodeQL SARIF file. A zero exit code confirms that SarifMark successfully parsed the
  SARIF input.
- **Markdown report generation**: the generated `docs/code_quality/codeql-quality.md`
  file is subsequently processed by PandocTool and WeasyprintTool; a successful PDF
  conversion confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet sarifmark --validate --results artifacts/sarifmark-self-validation.trx`
  executes SarifMark's internal test suite and writes TRX results consumed by ReqStream
  to verify the requirement `VHDLTest-OTS-SarifMark` is satisfied.
