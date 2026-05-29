## DemaConsulting.ReviewMark Verification

### Verification Approach

DemaConsulting.ReviewMark is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes ReviewMark to generate a review plan and review
report from `.reviewmark.yaml`. A passing CI step constitutes evidence that ReviewMark
correctly processed the review definition and rendered the output documents. ReviewMark
also provides an explicit `--validate` self-validation mode that writes TRX test results
consumed by ReqStream.

Enforcement mode (`--enforce`) is not yet active in the CI pipeline; it will be enabled
once the reviews branch of the repository is populated with review evidence. Verification
of enforcement behavior is therefore deferred until that branch is established.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (build-docs job). The
evidence store at `https://raw.githubusercontent.com/demaconsulting/VHDLTest/reviews/index.json`
must be accessible for the `--enforce` flag to be enabled; currently the pipeline runs
without `--enforce` until the reviews branch is populated.

### Acceptance Criteria

The ReviewMark step in the `build-docs` CI job completes with exit code 0. A passing step
constitutes evidence that ReviewMark correctly read `.reviewmark.yaml` and generated the
review plan and review report documents.

### Test Scenarios

- **Review plan generation**: `dotnet reviewmark --plan docs/code_review_plan/plan.md`
  produces a plan document listing all files grouped by review-set. Successful subsequent
  PandocTool and WeasyprintTool processing confirms the output is well-formed markdown.
- **Review report generation**: `dotnet reviewmark --report docs/code_review_report/report.md`
  produces a report showing review status for each file. Successful subsequent PDF
  conversion confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx`
  executes ReviewMark's internal test suite and writes TRX results consumed by ReqStream
  to verify the requirement `VHDLTest-OTS-ReviewMark` is satisfied.
