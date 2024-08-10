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

namespace DEMAConsulting.VHDLTest.Simulators;

/// <summary>
///     Simulator Factory Class
/// </summary>
public static class SimulatorFactory
{
    /// <summary>
    ///     Array of simulators
    /// </summary>
    private static readonly Simulator[] Simulators =
    [
        GhdlSimulator.Instance,
        ModelSimSimulator.Instance,
        VivadoSimulator.Instance,
        ActiveHdlSimulator.Instance,
        NvcSimulator.Instance
    ];

    /// <summary>
    ///     Get a simulator
    /// </summary>
    /// <param name="name">Simulator name</param>
    /// <returns>Simulator instance or null</returns>
    public static Simulator? Get(string? name = null)
    {
        // If the simulator is specified then use it
        if (name != null)
            return Array.Find(Simulators, s => s.SimulatorName.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        // Return the first available
        return Array.Find(Simulators, s => s.Available());
    }
}