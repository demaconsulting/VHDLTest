### SimulatorFactory

#### Verification Approach

`SimulatorFactory` is verified through unit tests in
`test/DEMAConsulting.VHDLTest.Tests/Simulators/SimulatorFactoryTests.cs`. Each test calls
`SimulatorFactory.Get` with a specific name string (including case variants) and asserts
that the returned instance is not null or is null for unknown names. No simulator process
is invoked; the tests exercise only the factory lookup logic.

#### Test Environment

N/A - standard test environment.

#### Acceptance Criteria

- All unit tests in `SimulatorFactoryTests.cs` pass with zero failures.
- `SimulatorFactory.Get` returns a non-null instance for all supported simulator names in
  any case combination.
- `SimulatorFactory.Get` returns null for unknown simulator names.
- `SimulatorFactory.Get` returns `MockSimulator.Instance` for the name `"mock"`.

#### Test Scenarios

**Get_GhdlSimulator_ReturnsGhdlSimulator**: Verifies that both `"ghdl"` and `"GHDL"`
resolve to the GHDL simulator instance, confirming case-insensitive matching.
This scenario is tested by `SimulatorFactory_Get_GhdlSimulator_ReturnsGhdlSimulator`.

**Get_ModelSimSimulator_ReturnsModelSimSimulator**: Verifies that both `"modelsim"` and
`"ModelSim"` resolve to the ModelSim simulator instance.
This scenario is tested by `SimulatorFactory_Get_ModelSimSimulator_ReturnsModelSimSimulator`.

**Get_VivadoSimulator_ReturnsVivadoSimulator**: Verifies that both `"vivado"` and `"Vivado"`
resolve to the Vivado simulator instance.
This scenario is tested by `SimulatorFactory_Get_VivadoSimulator_ReturnsVivadoSimulator`.

**Get_ActiveHDLSimulator_ReturnsActiveHDLSimulator**: Verifies that both `"activehdl"` and
`"ActiveHDL"` resolve to the Active-HDL simulator instance.
This scenario is tested by `SimulatorFactory_Get_ActiveHDLSimulator_ReturnsActiveHDLSimulator`.

**Get_NVCSimulator_ReturnsNVCSimulator**: Verifies that both `"nvc"` and `"NVC"` resolve
to the NVC simulator instance.
This scenario is tested by `SimulatorFactory_Get_NVCSimulator_ReturnsNVCSimulator`.

**Get_QuestaSimSimulator_ReturnsQuestaSimSimulator**: Verifies that both `"questasim"` and
`"QuestaSim"` resolve to the QuestaSim simulator instance.
This scenario is tested by `SimulatorFactory_Get_QuestaSimSimulator_ReturnsQuestaSimSimulator`.

**Get_UnknownSimulator_ReturnsNull**: Verifies that an unrecognised name returns null,
confirming the factory does not throw for unknown names.
This scenario is tested by `SimulatorFactory_Get_UnknownSimulator_ReturnsNull`.

**Get_MockSimulator_ReturnsMockSimulator**: Verifies that `"mock"`, `"Mock"`, and `"MOCK"`
all return the `MockSimulator` instance, confirming the mock is accessible by any case form
for test and validation purposes.
This scenario is tested by `SimulatorFactory_Get_MockSimulator_ReturnsMockSimulator`.
