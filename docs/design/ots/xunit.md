## xUnit v3 Integration Design

### Purpose

xUnit v3 (packages `xunit.v3` and `xunit.runner.visualstudio`) is the unit-testing framework
used by VHDLTest. It provides test discovery, execution, and result reporting for all test
projects. xUnit v3 is not deployed with VHDLTest; it is a build-time and CI-only dependency
included as a `PrivateAssets` reference in the test project.

### Features Used

- **Test discovery**: the `[Fact]` and `[Theory]` attributes mark test methods; xUnit v3
  discovers them automatically via the Visual Studio test runner integration.
- **Assertions**: `Assert` methods (e.g., `Assert.Equal`, `Assert.NotNull`, `Assert.Throws`)
  validate expected outcomes in every unit and integration test.
- **Test execution**: tests are run via `dotnet test`, which invokes xUnit's runner
  and collects results.
- **TRX result output**: the `--logger trx` option on `dotnet test` causes xunit.runner.visualstudio
  to write TRX result files consumed by ReqStream for requirements traceability.

### Integration Pattern

xUnit v3 is referenced in `test/DEMAConsulting.VHDLTest.Tests/DEMAConsulting.VHDLTest.Tests.csproj`
as a NuGet package dependency. Test execution is triggered by `dotnet test` in the CI
build job (`.github/workflows/build.yaml`) and locally via `build.ps1`. The `--logger trx`
flag produces TRX result files in the `artifacts/` directory. These TRX files are later
consumed by ReqStream to validate requirements traceability.

```csharp
// Example test using xUnit v3
[Fact]
public void SimulatorFactory_Get_GhdlSimulator_ReturnsGhdlSimulator()
{
    var simulator = SimulatorFactory.Get("ghdl");
    Assert.IsType<GhdlSimulator>(simulator);
}
```
