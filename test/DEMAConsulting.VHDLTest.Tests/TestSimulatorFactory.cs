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

namespace DEMAConsulting.VHDLTest.Tests;

/// <summary>
/// Tests for <see cref="SimulatorFactory"/> class.
/// </summary>
[TestClass]
public class TestSimulatorFactory
{
    /// <summary>
    /// Test querying the simulator factory for GHDL
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_Ghdl()
    {
        Assert.IsNotNull(SimulatorFactory.Get("ghdl"));
        Assert.IsNotNull(SimulatorFactory.Get("GHDL"));
    }

    /// <summary>
    /// Test querying the simulator factory for ModelSim
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_ModelSim()
    {
        Assert.IsNotNull(SimulatorFactory.Get("modelsim"));
        Assert.IsNotNull(SimulatorFactory.Get("ModelSim"));
    }

    /// <summary>
    /// Test querying the simulator factory for Vivado
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_Vivado()
    {
        Assert.IsNotNull(SimulatorFactory.Get("vivado"));
        Assert.IsNotNull(SimulatorFactory.Get("Vivado"));
    }

    /// <summary>
    /// Test querying the simulator factory for ActiveHDL
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_ActiveHDL()
    {
        Assert.IsNotNull(SimulatorFactory.Get("activehdl"));
        Assert.IsNotNull(SimulatorFactory.Get("ActiveHDL"));
    }

    /// <summary>
    /// Test querying the simulator factory for NVC
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_NVC()
    {
        Assert.IsNotNull(SimulatorFactory.Get("nvc"));
        Assert.IsNotNull(SimulatorFactory.Get("NVC"));
    }

    /// <summary>
    /// Test querying the simulator factory for an unknown simulator
    /// </summary>
    [TestMethod]
    public void Test_SimulatorFactory_Unknown()
    {
        Assert.IsNull(SimulatorFactory.Get("unknown"));
        Assert.IsNull(SimulatorFactory.Get("Unknown"));
    }
}