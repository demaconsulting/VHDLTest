## DemaConsulting.SonarMark Verification

### Verification Approach

DemaConsulting.SonarMark is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes SonarMark to retrieve SonarCloud quality data
and render a markdown report. A passing CI step constitutes evidence that SonarMark
correctly queried SonarCloud and rendered the report. SonarMark also provides an explicit
`--validate` self-validation mode that writes TRX test results consumed by ReqStream.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (`windows-latest`, build-docs job). A valid
`SONAR_TOKEN` must be available as a secret, and the SonarCloud project
`demaconsulting_VHDLTest` must have completed analysis via dotnet-sonarscanner before
the SonarMark step runs.

### Acceptance Criteria

The SonarMark step in the `build-docs` CI job completes with exit code 0. A passing step
constitutes evidence that SonarMark successfully retrieved quality gate status, issues,
and hotspots from SonarCloud and rendered the markdown report.

### Test Scenarios

- **Quality gate retrieval**: SonarMark queries the SonarCloud quality gate for the
  project. A zero exit code confirms the query succeeded and the result was rendered.
- **Issues and hotspot retrieval**: SonarMark fetches open issues and security hotspots.
  Successful markdown generation confirms both data sets were retrieved and rendered.
- **Markdown report generation**: the generated `docs/code_quality/sonar-quality.md`
  is subsequently processed by PandocTool and WeasyprintTool; a successful PDF conversion
  confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx`
  executes SonarMark's internal test suite and writes TRX results consumed by ReqStream
  to verify the requirement `VHDLTest-OTS-SonarMark` is satisfied.
