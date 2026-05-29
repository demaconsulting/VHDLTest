## DemaConsulting.SonarMark Integration Design

### Purpose

DemaConsulting.SonarMark retrieves quality gate status, metric data, issues, and hotspot
information from SonarCloud and renders it as a markdown report. The report is included in
the release code quality artifacts. SonarMark is not deployed with VHDLTest.

### Features Used

- **Quality gate retrieval**: fetches the SonarCloud quality gate result for the project.
- **Issues retrieval**: fetches open issues from SonarCloud.
- **Hotspot retrieval**: fetches security hotspot data from SonarCloud.
- **Markdown report generation**: renders quality data as a structured markdown document.
- **Self-validation**: `sonarmark --validate --results ...` executes SonarMark's own
  internal test suite and writes TRX results.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, SonarMark queries SonarCloud
after SonarScanner has completed its analysis in the `build` job:

```bash
dotnet sonarmark \
  --server https://sonarcloud.io \
  --project-key demaconsulting_VHDLTest \
  --branch "${{ github.ref_name }}" \
  --token "$SONAR_TOKEN" \
  --report docs/code_quality/sonar-quality.md \
  --report-depth 1
```

The generated markdown is converted to HTML by PandocTool and to PDF by WeasyprintTool.
