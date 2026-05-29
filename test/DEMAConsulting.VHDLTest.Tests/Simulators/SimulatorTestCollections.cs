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

namespace DEMAConsulting.VHDLTest.Tests.Simulators;

/// <summary>
///     Marks simulator test classes that read or write process-level environment variables
///     (e.g., <c>VHDLTEST_GHDL_PATH</c>, <c>VHDLTEST_NVC_PATH</c>,
///     <c>VHDLTEST_ACTIVEHDL_PATH</c>, <c>VHDLTEST_MODELSIM_PATH</c>) so that xUnit
///     serializes them and prevents race conditions between parallel test class runs.
/// </summary>
/// <remarks>
///     Environment variables are process-global mutable state. Tests that temporarily modify
///     them via try/finally must not run concurrently with other tests that read the same
///     variable, because the window between Set and Restore in one test can overlap with a
///     Read in another. Assigning all such test classes to this collection ensures they execute
///     serially with respect to each other while still running in parallel with unrelated
///     test classes.
/// </remarks>
[CollectionDefinition("SimulatorEnvVarTests", DisableParallelization = true)]
public sealed class SimulatorEnvVarTestsCollection
{
}
