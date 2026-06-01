## DemaConsulting.VersionMark Integration Design

### Purpose

DemaConsulting.VersionMark captures version metadata for all tools used in the CI pipeline
and aggregates them into a versions report. Each CI job independently records the versions
of the tools it uses; the `build-docs` job then publishes these captured records into a
single versions markdown document included in release artifacts. VersionMark is not
deployed with VHDLTest.

### Features Used

- **Version capture**: `versionmark --capture --job-id ... --output ...` queries each
  named tool for its version and writes the results to a JSON file.
- **Version publishing**: `versionmark --publish --report ... -- "artifacts/**/versionmark-*.json"`
  aggregates all captured JSON files and renders a markdown versions report.
- **Self-validation**: `versionmark --validate --results ...` executes VersionMark's own
  internal test suite and writes TRX results.
- **Lint**: `versionmark --lint` validates that tool version captures are consistent with
  the local tool manifest.

### Integration Pattern

Every CI job in `.github/workflows/build.yaml` runs `dotnet versionmark --capture`
immediately after installing tools. The `build-docs` job runs `dotnet versionmark --publish`
to produce `docs/build_notes/versions.md`, which is then converted to HTML and PDF. In
`lint.ps1` and `lint.sh`, `dotnet versionmark --lint` validates the captured version data.

```bash
dotnet versionmark --capture --job-id "build-docs" \
  --output "artifacts/versionmark-build-docs.json" -- \
  dotnet git node npm pandoc weasyprint sarifmark sonarmark reqstream buildmark versionmark reviewmark fileassert

dotnet versionmark --publish \
  --report docs/build_notes/versions.md \
  --report-depth 1 \
  -- "artifacts/**/versionmark-*.json"
```
