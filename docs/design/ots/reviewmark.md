## DemaConsulting.ReviewMark Integration Design

### Purpose

DemaConsulting.ReviewMark enforces the formal review process by verifying that every file
listed in a review-set (defined in `.reviewmark.yaml`) has associated review evidence in
the evidence store. It also generates a review plan document and a review report document.
When run with `--enforce`, ReviewMark fails the build if any file in a review-set lacks
current review evidence. ReviewMark is not deployed with VHDLTest.

### Version Used

DemaConsulting.ReviewMark 1.2.0 (dotnet tool `demaconsulting.reviewmark`).

### Features Used

- **Review plan generation**: `reviewmark --plan ...` produces a markdown document listing
  all files that require review, grouped by review-set.
- **Review report generation**: `reviewmark --report ...` produces a markdown document
  showing the review status of each file against the evidence store.
- **Enforcement mode**: `reviewmark --enforce` exits non-zero if any file in a review-set
  lacks evidence, making unreviewed artifacts a build-breaking condition.
- **Lint**: `reviewmark --lint` validates `.reviewmark.yaml` syntax.
- **Self-validation**: `reviewmark --validate --results ...` writes internal test TRX results.

### Integration Pattern

In the `build-docs` job of `.github/workflows/build.yaml`, ReviewMark is invoked with the
`.reviewmark.yaml` definition to generate plan and report documents:

```bash
dotnet reviewmark \
  --definition .reviewmark.yaml \
  --plan docs/code_review_plan/plan.md \
  --plan-depth 1 \
  --report docs/code_review_report/report.md \
  --report-depth 1
```

In `lint.ps1` and `lint.sh`, `dotnet reviewmark --lint` validates the review definition
file at development time. The evidence store is hosted on the `reviews` branch of the
repository and accessed via GitHub raw content URLs.
