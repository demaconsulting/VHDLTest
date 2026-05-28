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
    /// Test querying the simulator factory for GHDL
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_GhdlSimulator_ReturnsGhdlSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("ghdl"));
        Assert.NotNull(SimulatorFactory.Get("GHDL"));
    }

    /// <summary>
    /// Test querying the simulator factory for ModelSim
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_ModelSimSimulator_ReturnsModelSimSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("modelsim"));
        Assert.NotNull(SimulatorFactory.Get("ModelSim"));
    }

    /// <summary>
    /// Test querying the simulator factory for Vivado
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_VivadoSimulator_ReturnsVivadoSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("vivado"));
        Assert.NotNull(SimulatorFactory.Get("Vivado"));
    }

    /// <summary>
    /// Test querying the simulator factory for ActiveHDL
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_ActiveHDLSimulator_ReturnsActiveHDLSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("activehdl"));
        Assert.NotNull(SimulatorFactory.Get("ActiveHDL"));
    }

    /// <summary>
    /// Test querying the simulator factory for NVC
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_NVCSimulator_ReturnsNVCSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("nvc"));
        Assert.NotNull(SimulatorFactory.Get("NVC"));
    }

    /// <summary>
    /// Test querying the simulator factory for QuestaSim
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_QuestaSimSimulator_ReturnsQuestaSimSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("questasim"));
        Assert.NotNull(SimulatorFactory.Get("QuestaSim"));
    }

    /// <summary>
    /// Test querying the simulator factory for an unknown simulator
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_UnknownSimulator_ReturnsNull()
    {
        Assert.Null(SimulatorFactory.Get("unknown"));
        Assert.Null(SimulatorFactory.Get("Unknown"));
    }

    /// <summary>
    /// Test querying the simulator factory for the mock simulator
    /// </summary>
    [Fact]
    public void SimulatorFactory_Get_MockSimulator_ReturnsMockSimulator()
    {
        Assert.NotNull(SimulatorFactory.Get("mock"));
        Assert.NotNull(SimulatorFactory.Get("Mock"));
        Assert.NotNull(SimulatorFactory.Get("MOCK"));
    }
}
