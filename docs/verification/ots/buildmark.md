## DemaConsulting.BuildMark Verification

### Verification Approach

DemaConsulting.BuildMark is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes BuildMark to generate the build notes document.
A passing CI step constitutes evidence that BuildMark executed correctly and produced the
required output. BuildMark also provides an explicit `--validate` self-validation mode
that writes TRX test results consumed by ReqStream.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (`windows-latest`, build-docs job).

### Acceptance Criteria

The BuildMark step in the `build-docs` CI job completes with exit code 0. A passing
step constitutes evidence that BuildMark correctly queried GitHub Actions metadata and
rendered the build notes markdown document.

`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx` must exit
with code 0, and the written TRX file must contain all 5 self-test entries, each reported
as passed:

- `BuildMark_MarkdownReportGeneration`, satisfying `VHDLTest-OTS-BuildMark`.
- `BuildMark_GitIntegration`, satisfying `VHDLTest-OTS-BuildMark-GitIntegration`.
- `BuildMark_IssueTracking`, satisfying `VHDLTest-OTS-BuildMark-IssueTracking`.
- `BuildMark_KnownIssuesReporting`, satisfying `VHDLTest-OTS-BuildMark-KnownIssuesReporting`.
- `BuildMark_RulesRouting`, satisfying `VHDLTest-OTS-BuildMark-RulesRouting`.

### Test Scenarios

- **Build notes markdown generation**: `dotnet buildmark --build-version ... --report docs/build_notes.md`
  produces a markdown file containing the build version, workflow run details, and CI
  metadata. The generated file is subsequently processed by PandocTool and WeasyprintTool;
  a successful downstream PDF conversion confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`
  executes BuildMark's internal test suite and writes TRX results consumed by ReqStream
  to verify all five requirements are satisfied: `VHDLTest-OTS-BuildMark` (via
  `BuildMark_MarkdownReportGeneration`), `VHDLTest-OTS-BuildMark-GitIntegration` (via
  `BuildMark_GitIntegration`), `VHDLTest-OTS-BuildMark-IssueTracking` (via
  `BuildMark_IssueTracking`), `VHDLTest-OTS-BuildMark-KnownIssuesReporting` (via
  `BuildMark_KnownIssuesReporting`), and `VHDLTest-OTS-BuildMark-RulesRouting` (via
  `BuildMark_RulesRouting`).
