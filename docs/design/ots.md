# OTS Software Design

This document describes the integration and usage design for each off-the-shelf (OTS) software
item used by VHDLTest. OTS items are third-party libraries and tools that VHDLTest depends on
but does not own or modify. The purpose of this document is to record the version used, the
purpose, the features exercised, and the integration pattern for each OTS item.

## OTS Strategy

VHDLTest follows a policy of minimal OTS dependency: only libraries that provide significant
functionality that would be complex and error-prone to implement in-house are included. Each
OTS dependency is pinned to a specific version and reviewed for license compatibility. Runtime
dependencies are limited to those required at end-user execution time; build and analysis tools
are included only as `PrivateAssets` or as .NET global tools.

## Runtime Dependencies

The following OTS libraries are deployed with VHDLTest as runtime dependencies:

| Item | Version | Purpose |
| ---- | ------- | ------- |
| YamlDotNet | 18.0.0 | YAML deserialization of configuration files |
| DemaConsulting.TestResults | 1.7.0 | TRX test results file generation |

Detailed integration design for each runtime dependency is in its own document under
`docs/design/ots/`.

## Build and Analysis Tools

The following OTS tools are used in the build pipeline and are not deployed with VHDLTest:

| Item | Version | Purpose |
| ---- | ------- | ------- |
| xUnit v3 | — | Unit test execution framework |
| dotnet-sonarscanner | 11.2.1 | Static analysis and SonarCloud publishing |
| DemaConsulting.ReqStream | 1.10.0 | Requirements traceability enforcement |
| DemaConsulting.BuildMark | 1.2.2 | Build notes generation |
| DemaConsulting.VersionMark | 1.4.3 | Tool version capture |
| DemaConsulting.ReviewMark | 1.2.0 | Formal review enforcement |
| DemaConsulting.PandocTool | 3.9.0.2 | Markdown to HTML conversion |
| DemaConsulting.WeasyprintTool | 68.1.0 | HTML to PDF conversion |
| DemaConsulting.FileAssert | 0.3.0 | File comparison in CI |
| DemaConsulting.DictionaryMark | 0.1.0-beta.1 | Spell-checking enforcement |
| DemaConsulting.SarifMark | 1.3.2 | SARIF report processing |
| DemaConsulting.SonarMark | 1.5.0 | SonarCloud quality gate check |
