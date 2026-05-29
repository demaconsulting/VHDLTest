# OTS Verification

## OTS Verification Strategy

VHDLTest verifies OTS software items through two complementary approaches:

1. **Integration tests** — For runtime OTS dependencies (YamlDotNet, DemaConsulting.TestResults),
   integration tests in the main test project exercise the actual OTS API through the VHDLTest
   unit under test. These tests prove that the required OTS features work correctly within the
   VHDLTest integration context.

2. **CI pipeline validation** — For build and analysis tools, verification is provided by the
   tool's successful execution in CI. Non-zero exit codes from any tool step fail the pipeline.
   Each tool is validated implicitly every time the CI pipeline runs: a passing pipeline is
   evidence that all pipeline tools executed correctly. Several tools also provide an explicit
   `--validate` self-validation mode that writes TRX test results consumed by ReqStream.

## OTS Items

| Item | Type | Verification Approach |
| ---- | ---- | --------------------- |
| YamlDotNet | Runtime | Integration tests via ConfigDocument |
| DemaConsulting.TestResults | Runtime | Integration tests via TestResults |
| xUnit v3 | Build | Implicit — test suite execution |
| DemaConsulting.BuildMark | Build | CI pipeline execution and self-validation |
| DemaConsulting.VersionMark | Build | CI pipeline execution and self-validation |
| DemaConsulting.ReqStream | Build | CI pipeline execution and self-validation |
| DemaConsulting.SarifMark | Build | CI pipeline execution and self-validation |
| DemaConsulting.SonarMark | Build | CI pipeline execution and self-validation |
| DemaConsulting.ReviewMark | Build | CI pipeline execution and self-validation |
| DemaConsulting.PandocTool | Build | CI pipeline execution |
| DemaConsulting.WeasyprintTool | Build | CI pipeline execution |
| DemaConsulting.FileAssert | Build | CI pipeline execution |
| DemaConsulting.DictionaryMark | Build | CI pipeline execution |
| dotnet-sonarscanner | Build | CI pipeline execution |

Detailed verification evidence for each OTS item is in its own document under
`docs/verification/ots/`.
