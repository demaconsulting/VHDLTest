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

namespace DEMAConsulting.VHDLTest.Run;

/// <summary>
///     Abstracts low-level process execution to allow test injection of fake process behavior.
/// </summary>
/// <remarks>
///     The interface decouples <see cref="RunProcessor"/> from <see cref="RunProgram"/> so
///     that unit tests can supply a test double instead of launching real operating-system
///     processes. Production code uses <see cref="ProcessInvoker.Instance"/>.
/// </remarks>
public interface IProcessInvoker
{
    /// <summary>
    ///     Executes an application and returns its exit code and combined stdout+stderr output.
    /// </summary>
    /// <param name="workingDirectory">Working directory for the process.</param>
    /// <param name="application">Path or name of the executable to launch. Must not be pre-quoted.</param>
    /// <param name="arguments">Argument list; each element is passed as a separate argument.</param>
    /// <returns>A tuple of (exit code, combined stdout+stderr output).</returns>
    (int ExitCode, string Output) Execute(
        string workingDirectory,
        string application,
        IReadOnlyList<string> arguments);
}
