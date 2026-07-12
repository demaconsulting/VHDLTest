// Copyright (c) 2023 DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Tests for <see cref="SimulatorFactory"/> class.
/// </summary>
public class SimulatorFactoryTests
{
    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns a <see cref="GhdlSimulator"/>
    /// instance for both lower-case and upper-case spellings of the GHDL name, confirming that
    /// name matching is case-insensitive and the GHDL integration is registered correctly.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_GhdlSimulator_ReturnsGhdlSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns a GhdlSimulator instance for both cases of the GHDL name
        Assert.IsType<GhdlSimulator>(SimulatorFactory.Get("ghdl"));
        Assert.IsType<GhdlSimulator>(SimulatorFactory.Get("GHDL"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns a <see cref="ModelSimSimulator"/>
    /// instance for both lower-case and mixed-case spellings of the ModelSim name, confirming
    /// case-insensitive registration for the ModelSim integration.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_ModelSimSimulator_ReturnsModelSimSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns a ModelSimSimulator instance for both cases of the ModelSim name
        Assert.IsType<ModelSimSimulator>(SimulatorFactory.Get("modelsim"));
        Assert.IsType<ModelSimSimulator>(SimulatorFactory.Get("ModelSim"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns a <see cref="VivadoSimulator"/>
    /// instance for both lower-case and mixed-case spellings of the Vivado name, confirming
    /// case-insensitive registration for the Vivado integration.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_VivadoSimulator_ReturnsVivadoSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns a VivadoSimulator instance for both cases of the Vivado name
        Assert.IsType<VivadoSimulator>(SimulatorFactory.Get("vivado"));
        Assert.IsType<VivadoSimulator>(SimulatorFactory.Get("Vivado"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns an <see cref="ActiveHdlSimulator"/>
    /// instance for both lower-case and mixed-case spellings of the ActiveHDL name, confirming
    /// case-insensitive registration for the Active-HDL integration.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_ActiveHDLSimulator_ReturnsActiveHDLSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns an ActiveHdlSimulator instance for both cases of the ActiveHDL name
        Assert.IsType<ActiveHdlSimulator>(SimulatorFactory.Get("activehdl"));
        Assert.IsType<ActiveHdlSimulator>(SimulatorFactory.Get("ActiveHDL"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns an <see cref="NvcSimulator"/>
    /// instance for both lower-case and upper-case spellings of the NVC name, confirming
    /// case-insensitive registration for the NVC integration.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_NVCSimulator_ReturnsNVCSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns an NvcSimulator instance for both cases of the NVC name
        Assert.IsType<NvcSimulator>(SimulatorFactory.Get("nvc"));
        Assert.IsType<NvcSimulator>(SimulatorFactory.Get("NVC"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns a <see cref="QuestaSimSimulator"/>
    /// instance for both lower-case and mixed-case spellings of the QuestaSim name, confirming
    /// case-insensitive registration for the QuestaSim integration.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_QuestaSimSimulator_ReturnsQuestaSimSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns a QuestaSimSimulator instance for both cases of the QuestaSim name
        Assert.IsType<QuestaSimSimulator>(SimulatorFactory.Get("questasim"));
        Assert.IsType<QuestaSimSimulator>(SimulatorFactory.Get("QuestaSim"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns null for any name that does not
    /// match a registered simulator, satisfying the null-return contract for
    /// <c>VHDLTest-Simulators-SimulatorFactory-Unknown</c>. Callers must handle null to report
    /// a clear "simulator not found" error rather than a NullReferenceException.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_UnknownSimulator_ReturnsNull()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns null for unrecognized names
        Assert.Null(SimulatorFactory.Get("unknown"));
        Assert.Null(SimulatorFactory.Get("Unknown"));
    }

    /// <summary>
    /// Verifies that <see cref="SimulatorFactory.Get"/> returns a <see cref="MockSimulator"/>
    /// instance for all case variants of the mock name, confirming that the test-double
    /// simulator is registered and accessible for pipeline tests that must not invoke real
    /// simulator processes.
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_MockSimulator_ReturnsMockSimulator()
    {
        // Arrange: N/A - static factory requires no setup

        // Act / Assert: factory returns a MockSimulator instance for all cases of the mock name
        Assert.IsType<MockSimulator>(SimulatorFactory.Get("mock"));
        Assert.IsType<MockSimulator>(SimulatorFactory.Get("Mock"));
        Assert.IsType<MockSimulator>(SimulatorFactory.Get("MOCK"));
    }

    /// <summary>
    ///     Verifies that <see cref="SimulatorFactory.Get"/> performs auto-discovery when called
    ///     with null: returns the first available production simulator, or null when no simulator
    ///     is installed, and never returns <see cref="MockSimulator"/>.
    /// </summary>
    /// <remarks>
    ///     Satisfies <c>VHDLTest-Simulators-SimulatorFactory-AutoSelect</c>: auto-discovery returns either
    ///     a non-null Simulator instance (when at least one simulator is installed in the current
    ///     environment) or null (when no simulator is installed, as is typical in CI).
    ///     MockSimulator is excluded from auto-discovery results regardless of environment.
    ///     The expected result is independently recomputed from the exact declared order in
    ///     <see cref="SimulatorFactory"/> and compared by reference (singletons make reference
    ///     equality meaningful), so this test actually checks *first*, *available*, and
    ///     *ordering* rather than merely "not MockSimulator".
    /// </remarks>
    [Fact]
    public void SimulatorFactory_Get_WithNullName_ReturnsFirstAvailableOrNull()
    {
        // Arrange: independently recompute the expected result using the exact declared
        // order documented in SimulatorFactory.cs, so the assertion is environment-agnostic
        // (passes identically whether 0 or more simulators are actually installed) and
        // actually checks *first*, *available*, and *ordering* per the
        // VHDLTest-Simulators-SimulatorFactory-AutoSelect requirement.
        Simulator[] expectedOrder =
        [
            GhdlSimulator.Instance,
            ModelSimSimulator.Instance,
            QuestaSimSimulator.Instance,
            VivadoSimulator.Instance,
            ActiveHdlSimulator.Instance,
            NvcSimulator.Instance
        ];
        var expected = Array.Find(expectedOrder, s => s.Available());

        // Act: request auto-discovery by passing null
        var result = SimulatorFactory.Get(null);

        // Assert: result is the same singleton instance as the first available simulator in
        // declared order (reference equality is meaningful because simulators are singletons)
        Assert.Same(expected, result);
    }
}
