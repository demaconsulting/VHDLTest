## DemaConsulting.ReviewMark Integration Design

### Purpose

DemaConsulting.ReviewMark enforces the formal review process by verifying that every file
listed in a review-set (defined in `.reviewmark.yaml`) has associated review evidence in
the evidence store. It also generates a review plan document and a review report document.
When run with `--enforce`, ReviewMark fails the build if any file in a review-set lacks
current review evidence. ReviewMark is not deployed with VHDLTest.

### Features Used

- **Review plan generation**: `reviewmark --plan ...` produces a markdown document listing
  all files that require review, grouped by review-set.
- **Review report generation**: `reviewmark --report ...` produces a markdown document
  showing the review status of each file against the evidence store.
- **Lint**: `reviewmark --lint` validates `.reviewmark.yaml` syntax.
- **Self-validation**: `reviewmark --validate --results ...` writes internal test TRX results.
- **Version display**: `reviewmark --version` prints the installed ReviewMark version.
- **Help display**: `reviewmark --help` prints the command usage and available options.
- **Index scan**: `reviewmark --index <glob-path>` indexes PDF evidence files matching the
  given glob path into the evidence store.
- **Working-directory override**: `reviewmark --dir <directory>` sets the working directory
  used for default paths and glob scanning.
- **Enforce**: `reviewmark --enforce` exits with a non-zero code if any file in a
  review-set lacks current review evidence.
- **Elaborate**: `reviewmark --elaborate <id>` prints a markdown elaboration of the
  specified review set.
- **Depth flag**: `reviewmark --depth <#>` sets the default markdown heading depth applied
  to all generated documents (plan, report, and elaboration) when no per-document depth
  override is supplied.

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

Enforcement mode is planned for a future release when a reviews branch with PDF evidence is established.
