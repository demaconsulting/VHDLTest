## DemaConsulting.ReqStream Verification

### Verification Approach

DemaConsulting.ReqStream is verified through CI pipeline execution. The `build-docs` job
in `.github/workflows/build.yaml` invokes ReqStream in `--enforce` mode, which exits
non-zero if any requirement lacks a linked passing test. A passing `build-docs` job is
evidence that ReqStream correctly processed all requirements and all requirements have
linked passing tests. ReqStream also provides an explicit `--validate` self-validation
mode that writes TRX test results consumed by ReqStream itself.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Windows (`windows-latest`, build-docs job). All TRX
test result files from the build, integration, and validation jobs must be available as
artifacts before the ReqStream step runs.

### Acceptance Criteria

The ReqStream step in the `build-docs` CI job completes with exit code 0. This means
every requirement in `requirements.yaml` has at least one linked passing test recorded
in the collected TRX files.

### Test Scenarios

- **Requirements processing and traceability matrix generation**: `dotnet reqstream`
  processes `requirements.yaml` and all `artifacts/**/*.trx` files to generate a
  requirements document, justifications document, and trace matrix. Successful generation
  confirms that ReqStream read all inputs correctly.
- **Enforcement mode**: the `--enforce` flag ensures a non-zero exit code if any
  requirement has no linked passing test; a zero exit code confirms all requirements
  are satisfied.
- **Self-validation TRX output**: `dotnet reqstream --validate --results artifacts/reqstream-self-validation.trx`
  executes ReqStream's internal test suite and writes TRX results consumed by ReqStream to verify the
  requirements `VHDLTest-OTS-ReqStream` and `VHDLTest-OTS-ReqStream-Lint` are satisfied. The
  self-validation TRX includes the `ReqStream_Lint` test, which provides the passing evidence
  for `VHDLTest-OTS-ReqStream-Lint`.
- **Lint mode**: `dotnet reqstream --lint --requirements requirements.yaml` is invoked in the
  `quality-checks` CI job via `lint.ps1` and `lint.sh` to verify that all requirements YAML files
  conform to the ReqStream schema and contain no structural errors. A zero exit code confirms that
  all requirement files are well-formed and pass lint validation, satisfying requirement
  `VHDLTest-OTS-ReqStream-Lint`.
