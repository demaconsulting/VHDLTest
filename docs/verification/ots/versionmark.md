## DemaConsulting.VersionMark Verification

### Verification Approach

DemaConsulting.VersionMark is verified through CI pipeline execution. Every job in
`.github/workflows/build.yaml` invokes VersionMark to capture tool versions, and the
`build-docs` job invokes it to publish the aggregated versions report. A passing CI
pipeline is evidence that VersionMark executed correctly in all jobs. VersionMark also
provides an explicit `--validate` self-validation mode that writes TRX test results
consumed by ReqStream.

### Test Environment

CI/CD pipeline environment — GitHub Actions runners on Ubuntu, Windows, and macOS
(capture steps run in all build jobs; publish runs in the build-docs job).

### Acceptance Criteria

All VersionMark invocation steps in the CI pipeline complete with exit code 0. A passing
pipeline constitutes evidence that VersionMark correctly captured tool versions and
rendered the versions report.

### Test Scenarios

- **Version capture per job**: `dotnet versionmark --capture --job-id ... --output ...`
  is called in each CI job with the relevant tool names. A zero exit code confirms that
  VersionMark successfully queried each tool for its version and wrote the JSON output.
- **Versions report publication**: `dotnet versionmark --publish --report docs/build_notes/versions.md`
  aggregates all captured JSON files into a markdown report. Successful subsequent
  PandocTool and WeasyprintTool processing confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet versionmark --validate --results artifacts/versionmark-self-validation.trx`
  executes VersionMark's internal test suite and writes TRX results consumed by ReqStream
  to verify the requirements `VHDLTest-OTS-VersionMark-Capture`, `VHDLTest-OTS-VersionMark-Publish`,
  and `VHDLTest-OTS-VersionMark-Lint` are satisfied.
- **Lint mode**: `dotnet versionmark --lint` is invoked in the `quality-checks` CI job via `lint.ps1`
  to validate that tool version captures are consistent with the local tool manifest. A zero exit code
  confirms that all captured tool versions match the manifest declarations, satisfying requirement
  `VHDLTest-OTS-VersionMark-Lint`.
