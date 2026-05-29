## DemaConsulting.BuildMark Integration Design

### Purpose

DemaConsulting.BuildMark generates a build-notes markdown document from GitHub Actions
workflow run metadata. It queries the GitHub API to capture workflow details, timing, and
trigger information, producing a human-readable build notes file included in release
artifacts. BuildMark is a CI pipeline tool; it is not deployed with VHDLTest.

### Features Used

- **Build notes generation**: `buildmark --build-version ... --report ... --report-depth 1`
  renders a markdown document summarizing the build run, version, and CI metadata.
- **Self-validation**: `buildmark --validate --results ...` executes BuildMark's own
  internal test suite, writing results to a TRX file consumed by ReqStream.

### Integration Pattern

BuildMark is invoked in the `build-docs` job of `.github/workflows/build.yaml`. The
`--build-version` flag receives the release version from the workflow input, and
`--report docs/build_notes.md` writes the output markdown file. The generated file is
later converted to HTML by PandocTool and to PDF by WeasyprintTool. BuildMark also
runs `--validate` in the same job to produce a self-validation TRX result.

```bash
dotnet buildmark \
  --build-version "${{ inputs.version }}" \
  --report docs/build_notes.md \
  --report-depth 1
```
