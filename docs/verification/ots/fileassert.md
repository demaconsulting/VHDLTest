## DemaConsulting.FileAssert Verification

### Verification Approach

DemaConsulting.FileAssert is verified through CI pipeline execution. FileAssert is
invoked in CI pipeline steps to compare generated output files against committed baseline
files. A passing CI step constitutes evidence that FileAssert correctly compared the
files and confirmed they match.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner. Committed baseline files must be
present in the repository for comparison.

### Acceptance Criteria

All FileAssert invocation steps in the CI pipeline complete with exit code 0. A zero
exit code means the generated file matched the baseline; a non-zero exit code means
the files differ and the build fails.

### Test Scenarios

- **File comparison passes for matching files**: `dotnet fileassert expected-file.md generated-file.md`
  exits with code 0 when the generated file matches the committed baseline, confirming
  that no unintentional changes have occurred to the generated output.
- **Build failure on mismatch**: FileAssert exits non-zero when the generated file differs
  from the baseline, causing the CI step to fail and preventing divergent outputs from
  reaching release artifacts.
