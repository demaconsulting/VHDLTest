// Copyright (c) 2024 DEMA Consulting
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

using DEMAConsulting.VHDLTest.Cli;
using DEMAConsulting.VHDLTest.Run;
using DEMAConsulting.VHDLTest.Simulators;
using VHDLTestResult = DEMAConsulting.VHDLTest.Results.TestResult;

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
/// Tests for the <see cref="Simulator"/> base class.
/// </summary>
public class SimulatorTests
{
    /// <summary>
    /// Minimal concrete subclass of <see cref="Simulator"/> that exposes the protected
    /// <see cref="Simulator.Where"/> method for unit testing.
    /// </summary>
    private sealed class TestableSimulator : Simulator
    {
        /// <summary>
        /// Initializes a new instance with a null simulator path.
        /// </summary>
        public TestableSimulator() : base("Test", null)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified simulator path.
        /// </summary>
        public TestableSimulator(string? path) : base("Test", path)
        {
        }

        /// <summary>
        /// Delegates to the protected <see cref="Simulator.Where"/> method.
        /// </summary>
        /// <param name="application">Application name to locate.</param>
        /// <returns>Full path to the executable, or null if not found.</returns>
        public static string? CallWhere(string application) => Where(application);

        /// <inheritdoc />
        public override RunResults Compile(Context context, Options options) =>
            throw new InvalidOperationException("SimulatorPath is null.");

        /// <inheritdoc />
        public override VHDLTestResult Test(Context context, Options options, string test) =>
            throw new InvalidOperationException("SimulatorPath is null.");
    }

    /// <summary>
    /// Test that Where returns null when searching for an executable that does not exist
    /// on the system PATH.
    /// </summary>
    [Fact]
    public void Simulator_Where_UnknownExecutable_ReturnsNull()
    {
        // Arrange: choose an executable name that will never exist on any PATH
        var unknownExecutable = "vhdltest-nonexistent-" + Guid.NewGuid().ToString("N");

        // Act: search the system PATH for the unknown executable
        var result = TestableSimulator.CallWhere(unknownExecutable);

        // Assert: the executable is not found and null is returned
        Assert.Null(result);
    }

    /// <summary>
    /// Test that Where returns a non-null path when searching for an executable that is
    /// guaranteed to be present on the system PATH in the CI environment.
    /// Satisfies <c>VHDLTest-Simulators-Simulator-Where</c>: PATH search succeeds and returns
    /// a non-null path when the named executable is installed.
    /// </summary>
    [Fact]
    public void Simulator_Where_ExistingExecutable_ReturnsNonNull()
    {
        // Arrange: "dotnet" is guaranteed to be on PATH in the CI environment

        // Act: search the system PATH for the dotnet executable
        var result = TestableSimulator.CallWhere("dotnet");

        // Assert: the executable is found and a non-null path is returned
        Assert.NotNull(result);
    }

    /// <summary>
    /// Test that Available() returns true when SimulatorPath is non-null.
    /// </summary>
    [Fact]
    public void Simulator_Available_WithNonNullPath_ReturnsTrue()
    {
        // Arrange: construct a simulator with a non-null path
        var simulator = new TestableSimulator("/some/path");

        // Act: check availability
        var result = simulator.Available();

        // Assert: non-null path means available
        Assert.True(result);
    }
}
