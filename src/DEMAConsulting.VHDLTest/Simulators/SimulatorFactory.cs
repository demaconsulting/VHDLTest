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
///     Centralizes access to all supported VHDL simulator implementations.
/// </summary>
/// <remarks>
///     Acts as the single point of simulator selection for the rest of the application:
///     callers request a simulator by name (or omit the name for auto-discovery) and
///     receive the matching <see cref="Simulator"/> instance without knowing which
///     concrete type is returned. Adding a new simulator requires only registering
///     its singleton in the <see cref="Simulators"/> array. Stateless static class;
///     thread-safe by construction.
/// </remarks>
public static class SimulatorFactory
{
    /// <summary>
    ///     Production simulators available for auto-discovery and named lookup.
    /// </summary>
    /// <remarks>
    ///     Contains the six production simulator singletons in priority order for
    ///     auto-discovery: <see cref="GhdlSimulator"/>, <see cref="ModelSimSimulator"/>,
    ///     <see cref="QuestaSimSimulator"/>, <see cref="VivadoSimulator"/>,
    ///     <see cref="ActiveHdlSimulator"/>, and <see cref="NvcSimulator"/>.
    ///     <see cref="MockSimulator"/> is intentionally excluded: it is a test-only
    ///     simulator that must be requested explicitly by name and must never be
    ///     silently returned during auto-discovery.
    /// </remarks>
    private static readonly Simulator[] Simulators =
    [
        GhdlSimulator.Instance,
        ModelSimSimulator.Instance,
        QuestaSimSimulator.Instance,
        VivadoSimulator.Instance,
        ActiveHdlSimulator.Instance,
        NvcSimulator.Instance
    ];

    /// <summary>
    ///     Returns the simulator matching the specified name, or the first available
    ///     simulator when no name is given.
    /// </summary>
    /// <remarks>
    ///     Name matching is case-insensitive. The special name "mock" always returns
    ///     <see cref="MockSimulator"/> regardless of whether it is installed;
    ///     MockSimulator is excluded from the production array and can only be reached
    ///     by this explicit path. When <paramref name="name"/> is null the method
    ///     performs auto-discovery: it iterates the production <see cref="Simulators"/>
    ///     array in declaration order and returns the first entry whose
    ///     <see cref="Simulator.Available()"/> method returns true. Stateless and
    ///     thread-safe; no mutable state is accessed.
    /// </remarks>
    /// <param name="name">
    ///     Simulator name (case-insensitive). Pass null to auto-discover the first
    ///     available installed simulator. Pass "mock" to obtain the
    ///     <see cref="MockSimulator"/> instance for self-test purposes. Pass any
    ///     other recognized name (e.g. "GHDL", "NVC", "ModelSim") to obtain that
    ///     simulator's singleton. Unrecognized names return null.
    /// </param>
    /// <returns>
    ///     <list type="bullet">
    ///         <item><description>
    ///             The <see cref="MockSimulator"/> singleton when <paramref name="name"/>
    ///             equals "mock" (case-insensitive).
    ///         </description></item>
    ///         <item><description>
    ///             The matching production simulator singleton when <paramref name="name"/>
    ///             is a recognized simulator name.
    ///         </description></item>
    ///         <item><description>
    ///             The first available (installed) production simulator when
    ///             <paramref name="name"/> is null and at least one simulator is installed.
    ///         </description></item>
    ///         <item><description>
    ///             Null when <paramref name="name"/> is null and no simulator is installed,
    ///             or when <paramref name="name"/> is an unrecognized non-null value.
    ///         </description></item>
    ///     </list>
    /// </returns>
    public static Simulator? Get(string? name = null)
    {
        // Only return the mock simulator if explicitly requested
        if (name?.Equals("mock", StringComparison.OrdinalIgnoreCase) == true)
        {
            return MockSimulator.Instance;
        }

        // If the simulator is specified then use it
        if (name != null)
        {
            return Array.Find(Simulators, s => s.SimulatorName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // Return the first available
        return Array.Find(Simulators, s => s.Available());
    }
}
