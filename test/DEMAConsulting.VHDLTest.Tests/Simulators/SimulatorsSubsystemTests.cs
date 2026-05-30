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

using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Subsystem integration tests for the Simulators subsystem.
/// These tests verify that <see cref="SimulatorFactory"/> and the simulator
/// processors work together to select and classify simulator output.
/// </summary>
public class SimulatorsSubsystemTests
{
    /// <summary>
    ///     Test that the factory returns the GHDL simulator by name, and that the
    ///     GHDL compile processor correctly classifies error output.
    /// </summary>
    /// <remarks>
    ///     This test exercises the Error severity category end-to-end at the subsystem level.
    ///     Error classification is the highest severity and the most critical to verify:
    ///     misclassifying an error as text would silently allow a failed compile to proceed,
    ///     producing misleading test results. Confirming both factory selection and processor
    ///     classification in a single test validates that the subsystem integration path for
    ///     the most important severity level is correct.
    /// </remarks>
    [Fact]
    public void SimulatorsSubsystem_GetSimulatorAndProcessCompileOutput_WithErrorOutput_ClassifiesAsError()
    {
        // Arrange - obtain GHDL simulator via the factory to confirm factory selection
        var simulator = SimulatorFactory.Get("GHDL");

        // Act - access GHDL compile processor directly by class name and classify error output
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "design.vhd:10:5: error: undefined identifier 'x'",
            1);

        // Assert - factory returned GHDL, its concrete type is correct, and the processor
        // classified the diagnostic line as an error
        Assert.NotNull(simulator);
        Assert.IsType<GhdlSimulator>(simulator);
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Contains(results.Lines, line => line.Type == RunLineType.Error);
    }

    /// <summary>
    ///     Test that the factory returns null for an unrecognized simulator name.
    /// </summary>
    /// <remarks>
    ///     Verifying the null-return contract for unknown simulator names is important because
    ///     callers rely on it to distinguish "simulator not registered" from other failure modes.
    ///     A factory that throws instead of returning null would require every caller to add
    ///     exception handling; a factory that returns a default instance would silently route
    ///     tests to the wrong simulator.
    /// </remarks>
    [Fact]
    public void SimulatorsSubsystem_GetSimulatorByName_WithUnknownName_ReturnsNull()
    {
        // Arrange - N/A

        // Act - attempt to obtain an unknown simulator via the factory
        var simulator = SimulatorFactory.Get("unknown-simulator");

        // Assert - unknown simulator name yields null
        Assert.Null(simulator);
    }

    /// <summary>
    ///     Test that the NVC test processor correctly classifies clean output as text.
    /// </summary>
    /// <remarks>
    ///     Clean (no-diagnostic) output is the most common production result and forms the
    ///     baseline for all severity classifications. If clean output were misclassified as a
    ///     warning or error, every successful test run would be reported as a failure. Testing
    ///     the NVC processor separately from the GHDL error test also confirms that factory
    ///     selection and processor correctness work for a second simulator, providing broader
    ///     subsystem coverage.
    /// </remarks>
    [Fact]
    public void SimulatorsSubsystem_ProcessOutput_NvcWithCleanOutput_ClassifiesAsText()
    {
        // Arrange - obtain NVC simulator via the factory to confirm factory selection
        var simulator = SimulatorFactory.Get("NVC");

        // Act - access NVC test processor directly by class name and classify clean output
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "Simulation complete",
            0);

        // Assert - factory returned NVC, its concrete type is correct, and the processor
        // classified the clean output line as text
        Assert.NotNull(simulator);
        Assert.IsType<NvcSimulator>(simulator);
        Assert.Equal(RunLineType.Text, results.Summary);
    }

    /// <summary>
    ///     Test that the GHDL compile processor correctly classifies warning output.
    /// </summary>
    /// <remarks>
    ///     The Warning severity category must be verified at the subsystem integration level to
    ///     confirm that the subsystem-level classification contract covers all diagnostic severities.
    ///     A compile warning is less severe than an error but still meaningful to the user;
    ///     misclassifying a warning as text would hide potentially important VHDL analysis diagnostics.
    ///     Using the GHDL compile processor ensures this is tested against a real simulator's
    ///     classification rules rather than a mock.
    /// </remarks>
    [Fact]
    public void SimulatorsSubsystem_ProcessOutput_GhdlWithWarningOutput_ClassifiesAsWarning()
    {
        // Arrange - obtain GHDL simulator via the factory to confirm factory selection
        var simulator = SimulatorFactory.Get("GHDL");

        // Act - access GHDL compile processor directly by class name and classify warning output
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "design.vhd:5:1:warning: signal 'unused' is never read",
            0);

        // Assert - factory returned GHDL, its concrete type is correct, and the processor
        // classified the diagnostic line as a warning
        Assert.NotNull(simulator);
        Assert.IsType<GhdlSimulator>(simulator);
        Assert.Equal(RunLineType.Warning, results.Summary);
        Assert.Contains(results.Lines, line => line.Type == RunLineType.Warning);
    }

    /// <summary>
    ///     Test that the NVC test processor correctly classifies info output.
    /// </summary>
    /// <remarks>
    ///     The Info severity category must be verified at the subsystem integration level to confirm
    ///     that all four diagnostic severity levels (Text, Info, Warning, Error) are exercised
    ///     end-to-end through the subsystem interface. Info messages represent simulator-generated
    ///     notes that are informational rather than indicative of a problem; misclassifying them
    ///     as text would lose the distinction between plain output and simulator-generated notes.
    ///     Using the NVC test processor confirms the NVC note pattern is correctly classified
    ///     through the full subsystem path.
    /// </remarks>
    [Fact]
    public void SimulatorsSubsystem_ProcessOutput_NvcWithInfoOutput_ClassifiesAsInfo()
    {
        // Arrange - obtain NVC simulator via the factory to confirm factory selection
        var simulator = SimulatorFactory.Get("NVC");

        // Act - access NVC test processor directly by class name and classify info output
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "Simulation Note: test completed normally",
            0);

        // Assert - factory returned NVC, its concrete type is correct, and the processor
        // classified the note line as info
        Assert.NotNull(simulator);
        Assert.IsType<NvcSimulator>(simulator);
        Assert.Equal(RunLineType.Info, results.Summary);
        Assert.Contains(results.Lines, line => line.Type == RunLineType.Info);
    }
}
