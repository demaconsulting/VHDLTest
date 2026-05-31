## DemaConsulting.FileAssert Verification

### Verification Approach

DemaConsulting.FileAssert is verified through CI pipeline execution and self-validation.
FileAssert is invoked in CI pipeline steps to compare generated output files against committed
baseline files. A passing CI step constitutes evidence that FileAssert correctly compared the
files and confirmed they match. FileAssert also provides a `--validate` self-validation mode
that exercises its built-in test suite; `dotnet fileassert --validate --results` is invoked in
the `build-docs` job of `.github/workflows/build.yaml` and writes a TRX result file consumed
by ReqStream for requirements traceability evidence.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner. Committed baseline files must be
present in the repository for comparison.

### Acceptance Criteria

All FileAssert invocation steps in the CI pipeline complete with exit code 0. A zero
exit code means the generated file matched the baseline; a non-zero exit code means
the files differ and the build fails.

### Test Scenarios

- **File comparison passes for matching files**: `FileAssert_Results` exercises the primary
  comparison capability — `dotnet fileassert expected-file.md generated-file.md` exits with
  code 0 when the generated file matches the committed baseline, confirming that no
  unintentional changes have occurred to the generated output.
- **Build failure on mismatch**: FileAssert exits non-zero when the generated file differs
  from the baseline, causing the CI step to fail and preventing divergent outputs from
  reaching release artifacts.
- **File existence verification**: `FileAssert_Exists` verifies that FileAssert correctly
  checks whether an expected output file was created by a preceding pipeline step and exits
  non-zero if the file is absent, providing evidence that existence checking is operational.
- **File content searching**: `FileAssert_Contains` verifies that FileAssert correctly searches
  for expected text content within a generated file and exits non-zero if the expected content
  is not found, providing evidence that content verification is operational.
- **FileAssert_SelfValidation**: `dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx`
  executes FileAssert's internal test suite in the `build-docs` CI job and writes TRX results
  consumed by ReqStream, providing traceability evidence that FileAssert's comparison,
  existence-checking, and content-searching capabilities are operational.
