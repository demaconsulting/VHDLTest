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
- **Git integration**: BuildMark reads commit and branch metadata from the local git
  repository to enrich the generated build notes.
- **Issue tracking**: BuildMark queries the GitHub API for issues referenced by the build
  run and includes them in the generated build notes document.
- **Known-issues reporting**: `buildmark --include-known-issues` includes a known-issues
  section in the generated report.
- **Rules routing**: BuildMark routes report content according to configured rules,
  determining which sections are included for a given build context.

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

### Error Handling

BuildMark propagates GitHub API failures (authentication errors, rate limiting, or network
failures while querying workflow, issue, or commit metadata) as a non-zero process exit code
with a diagnostic message; the CI job step fails and no build notes file is written. When
local git metadata (commit or branch information) is unavailable — for example, when running
outside a git repository or in a shallow clone missing the required history — the affected
build-notes section is omitted rather than causing the process to fail, so a report can still
be generated with reduced content. `--validate` self-validation failures (any of the five
self-tests failing) are reported as failed entries in the written TRX file and cause a
non-zero exit code, which the `build-docs` CI job surfaces as a failed step.
