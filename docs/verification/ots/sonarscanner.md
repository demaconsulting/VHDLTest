## dotnet-sonarscanner Verification

### Verification Approach

dotnet-sonarscanner is verified through CI pipeline execution. The `build` job in
`.github/workflows/build.yaml` invokes dotnet-sonarscanner to wrap the `dotnet build`
and `dotnet test` steps and publish analysis results to SonarCloud. A passing CI step
constitutes evidence that dotnet-sonarscanner correctly configured the analysis context,
collected code metrics and coverage data, and published results to SonarCloud.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Ubuntu, Windows, and macOS (build
job matrix). A valid `SONAR_TOKEN` must be available as a secret, and the SonarCloud
project `demaconsulting_VHDLTest` must be configured to accept analysis submissions.

### Acceptance Criteria

The dotnet-sonarscanner `begin` and `end` steps in the CI build job complete with exit
code 0 on all platform combinations in the matrix. A passing pipeline constitutes evidence
that dotnet-sonarscanner correctly collected analysis data and published it to SonarCloud.

### Test Scenarios

- **Analysis begin**: `dotnet dotnet-sonarscanner begin /k:"demaconsulting_VHDLTest" ...`
  configures the analysis context. A zero exit code confirms that dotnet-sonarscanner
  accepted all configuration parameters and initialized the analysis session.
- **Analysis end and publication**: `dotnet dotnet-sonarscanner end /d:sonar.token=...`
  finalizes analysis and publishes results. A zero exit code confirms that metrics,
  coverage data, and findings were successfully uploaded to SonarCloud.
- **Code coverage integration**: OpenCover XML coverage files produced by `dotnet test`
  are referenced via `sonar.cs.opencover.reportsPaths`; successful SonarCloud analysis
  containing coverage metrics confirms that dotnet-sonarscanner correctly collected
  coverage data.
- **Cross-platform analysis**: analysis runs on Ubuntu, Windows, and macOS in a matrix
  strategy, confirming that dotnet-sonarscanner operates correctly across all supported
  CI platforms.
