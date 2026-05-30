## DemaConsulting.ReviewMark Verification

### Verification Approach

DemaConsulting.ReviewMark is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes ReviewMark to generate a review plan and review
report from `.reviewmark.yaml`. A passing CI step constitutes evidence that ReviewMark
correctly processed the review definition and rendered the output documents. ReviewMark
also provides an explicit `--validate` self-validation mode that writes TRX test results
consumed by ReqStream.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (build-docs job).

### Acceptance Criteria

The ReviewMark step in the `build-docs` CI job completes with exit code 0. A passing step
constitutes evidence that ReviewMark correctly read `.reviewmark.yaml` and generated the
review plan and review report documents.

### Test Scenarios

- **ReviewMark_CoverageReport**: The CI pipeline invokes
  `dotnet reviewmark --definition .reviewmark.yaml --plan docs/code_review_plan/plan.md
  --plan-depth 1 --report docs/code_review_report/report.md --report-depth 1` in the
  `build-docs` job. A passing step confirms ReviewMark successfully read `.reviewmark.yaml`
  and generated the review plan and report documents, reporting file coverage across all
  review-sets.
- **Review plan generation**: `dotnet reviewmark --plan docs/code_review_plan/plan.md`
  produces a plan document listing all files grouped by review-set. Successful subsequent
  PandocTool and WeasyprintTool processing confirms the output is well-formed markdown.
- **Review report generation**: `dotnet reviewmark --report docs/code_review_report/report.md`
  produces a report showing review status for each file. Successful subsequent PDF
  conversion confirms the output is well-formed markdown.
- **Self-validation TRX output**: `dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx`
  executes ReviewMark's internal test suite and writes TRX results consumed by ReqStream.
- **ReviewMark_LintPasses**: `dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx`
  in the `build-docs` job executes ReviewMark's internal test suite. The `ReviewMark_LintPasses`
  test ID is written to the TRX result file and consumed by ReqStream to confirm that
  ReviewMark's lint-pass validation logic is operational.
