# OTS Software Design

This document describes the integration and usage design for each off-the-shelf (OTS) software
item used by VHDLTest. OTS items are third-party libraries and tools that VHDLTest depends on
but does not own or modify. The purpose of this document is to record the purpose, the features
exercised, and the integration pattern for each OTS item.

## OTS Strategy

VHDLTest follows a policy of minimal OTS dependency: only libraries that provide significant
functionality that would be complex and error-prone to implement in-house are included. Each
OTS dependency is pinned to a specific version and reviewed for license compatibility. Runtime
dependencies are limited to those required at end-user execution time; build and analysis tools
are included only as `PrivateAssets` or as .NET global tools.

## Runtime Dependencies

The following OTS libraries are deployed with VHDLTest as runtime dependencies:

| Item | Purpose |
| ---- | ------- |
| YamlDotNet | YAML deserialization of configuration files |
| DemaConsulting.TestResults | TRX test results file generation |

Detailed integration design for each runtime dependency is in its own document under
`docs/design/ots/`.

## Build and Analysis Tools

The following OTS tools are used in the build pipeline and are not deployed with VHDLTest:

| Item | Purpose |
| ---- | ------- |
| xUnit v3 | Unit test execution framework |
| dotnet-sonarscanner | Static analysis and SonarCloud publishing |
| DemaConsulting.ReqStream | Requirements traceability enforcement |
| DemaConsulting.BuildMark | Build notes generation |
| DemaConsulting.VersionMark | Tool version capture |
| DemaConsulting.ReviewMark | Formal review enforcement |
| DemaConsulting.PandocTool | Markdown to HTML conversion |
| DemaConsulting.WeasyprintTool | HTML to PDF conversion |
| DemaConsulting.FileAssert | File comparison in CI |
| DemaConsulting.DictionaryMark | Spell-checking enforcement |
| DemaConsulting.SarifMark | SARIF report processing |
| DemaConsulting.SonarMark | SonarCloud quality gate check |
