## dotnet-sonarscanner Integration Design

### Purpose

dotnet-sonarscanner integrates the .NET build pipeline with SonarCloud for continuous
static analysis and code quality monitoring. It wraps the build by issuing a `begin`
command before `dotnet build` and an `end` command after `dotnet test`, collecting code
metrics, coverage data, and static analysis findings and publishing them to SonarCloud.
dotnet-sonarscanner is not deployed with VHDLTest.

### Features Used

- **Analysis begin**: `dotnet-sonarscanner begin` configures the analysis context,
  specifying the project key, organization, SonarCloud token, coverage paths, and
  scanner settings.
- **Analysis end**: `dotnet-sonarscanner end` finalizes analysis, uploads metrics,
  coverage, and findings to SonarCloud.
- **Code coverage integration**: the `sonar.cs.opencover.reportsPaths` setting points
  SonarCloud to the OpenCover XML coverage files produced by `dotnet test`.

### Integration Pattern

In the `build` job of `.github/workflows/build.yaml`, dotnet-sonarscanner wraps the
`dotnet build` and `dotnet test` steps:

```bash
# Before build
dotnet dotnet-sonarscanner begin \
  /k:"demaconsulting_VHDLTest" \
  /o:"demaconsulting" \
  /d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
  /d:sonar.host.url="https://sonarcloud.io" \
  /d:sonar.cs.opencover.reportsPaths=**/*.opencover.xml \
  /d:sonar.scanner.scanAll=false

# ... dotnet build and dotnet test run here ...

# After test
dotnet dotnet-sonarscanner end \
  /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
```

The analysis runs on Ubuntu, Windows, and macOS in a matrix strategy so that SonarCloud
receives coverage data from all supported platforms.
