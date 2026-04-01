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
[TestClass]
public class SimulatorsSubsystemTests
{
    /// <summary>
    /// Test that the factory returns the GHDL simulator by name, and that the
    /// simulator's compile processor correctly classifies error output.
    /// </summary>
    [TestMethod]
    public void SimulatorsSubsystem_GetSimulatorAndProcessCompileOutput_WithErrorOutput_ClassifiesAsError()
    {
        // Arrange - obtain GHDL simulator via the factory
        var simulator = SimulatorFactory.Get("GHDL");

        // Act - use the simulator's compile processor to classify error output
        var results = GhdlSimulator.CompileProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "design.vhd:10:5: error: undefined identifier 'x'",
            1);

        // Assert - factory returned GHDL and processor classified the line as an error
        Assert.IsNotNull(simulator);
        Assert.AreEqual("GHDL", simulator.SimulatorName);
        Assert.AreEqual(RunLineType.Error, results.Summary);
        Assert.IsTrue(results.Lines.Any(l => l.Type == RunLineType.Error));
    }

    /// <summary>
    /// Test that the factory returns null for an unrecognized simulator name, and
    /// that the NVC simulator's test processor correctly classifies clean output.
    /// </summary>
    [TestMethod]
    public void SimulatorsSubsystem_GetUnknownSimulatorAndProcessCleanOutput_ReturnsNullAndClassifiesText()
    {
        // Arrange - attempt to obtain an unknown simulator via the factory
        var simulator = SimulatorFactory.Get("unknown-simulator");

        // Act - use the NVC test processor independently to classify clean output
        var results = NvcSimulator.TestProcessor.Parse(
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2024, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            "Simulation complete",
            0);

        // Assert - unknown simulator yields null; processor classified clean output as text
        Assert.IsNull(simulator);
        Assert.AreEqual(RunLineType.Text, results.Summary);
    }
}
