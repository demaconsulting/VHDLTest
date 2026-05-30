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
    /// Test that the factory returns the GHDL simulator by name, and that the
    /// factory-returned simulator's compile processor correctly classifies error output.
    /// </summary>
    [Fact]
    public void SimulatorsSubsystem_GetSimulatorAndProcessCompileOutput_WithErrorOutput_ClassifiesAsError()
    {
        // Arrange - obtain GHDL simulator via the factory and cast to access its processor
        var simulator = SimulatorFactory.Get("GHDL");
        Assert.NotNull(simulator);
        Assert.IsType<GhdlSimulator>(simulator);

        // Act - use the factory-returned simulator's compile processor to classify error output
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "design.vhd:10:5: error: undefined identifier 'x'",
            1);

        // Assert - factory returned GHDL and processor classified the line as an error
        Assert.Equal(RunLineType.Error, results.Summary);
        Assert.Contains(results.Lines, line => line.Type == RunLineType.Error);
    }

    /// <summary>
    /// Test that the factory returns null for an unrecognized simulator name.
    /// </summary>
    [Fact]
    public void SimulatorsSubsystem_GetUnknownSimulatorByName_ReturnsNull()
    {
        // Arrange - N/A

        // Act - attempt to obtain an unknown simulator via the factory
        var simulator = SimulatorFactory.Get("unknown-simulator");

        // Assert - unknown simulator name yields null
        Assert.Null(simulator);
    }

    /// <summary>
    /// Test that the NVC simulator's test processor correctly classifies clean output as text.
    /// </summary>
    [Fact]
    public void SimulatorsSubsystem_NvcProcessorWithCleanOutput_ClassifiesAsText()
    {
        // Arrange - obtain NVC simulator via the factory and cast to access its processor
        var simulator = SimulatorFactory.Get("NVC");
        Assert.NotNull(simulator);
        Assert.IsType<NvcSimulator>(simulator);

        // Act - use the factory-returned simulator's test processor to classify clean output
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "Simulation complete",
            0);

        // Assert - NVC processor classified clean output as text
        Assert.Equal(RunLineType.Text, results.Summary);
    }
}
