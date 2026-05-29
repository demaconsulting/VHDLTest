## xUnit v3 Verification

### Verification Approach

xUnit v3 is verified implicitly through its own operation: the test suite for VHDLTest
runs entirely under xUnit v3. A passing `dotnet test` execution is evidence that xUnit v3
correctly discovered and executed all test methods and reported results. The CI build job
runs `dotnet test` on Ubuntu, Windows, and macOS to confirm cross-platform compatibility.

### Test Environment

CI/CD pipeline environment — GitHub Actions runner on Ubuntu, Windows, and macOS with
.NET 8, 9, and 10 in a matrix strategy.

### Acceptance Criteria

The `dotnet test` step in the CI build job completes with exit code 0 on all platform
and runtime combinations in the matrix. A passing test run constitutes evidence that
xUnit v3 correctly executed and reported test results.

### Test Scenarios

- **Test discovery and execution**: all test methods annotated with `[Fact]` or `[Theory]`
  in the test project are discovered and executed, confirming xUnit's discovery mechanism
  is functional.
- **TRX result output**: the `--logger trx` option produces TRX files consumed by
  ReqStream, confirming that xUnit's runner integration correctly writes structured
  result data.
- **Cross-platform execution**: tests pass on Ubuntu, Windows, and macOS runner images,
  confirming xUnit v3 operates correctly across all supported deployment platforms.
