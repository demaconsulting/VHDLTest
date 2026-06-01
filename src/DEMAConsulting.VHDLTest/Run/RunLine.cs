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
///     Immutable positional record that encapsulates a single simulator output line together
///     with its classification, as a strongly typed value passed between
///     <see cref="RunProcessor"/> and <see cref="RunResults"/>.
/// </summary>
/// <remarks>
///     RunLine instances are created by <see cref="RunProcessor.Parse"/> for every line of
///     captured simulator output. Using a dedicated record rather than a raw string avoids
///     repeated pattern matching in downstream logic; the classification is applied once and
///     carried forward throughout the processing pipeline. This type is immutable and
///     thread-safe; instances may be shared freely across threads.
/// </remarks>
/// <param name="Type">
///     Classification assigned to this line by <see cref="RunProcessor.Parse"/> through
///     <see cref="RunLineRule"/> pattern matching. Lines that match no rule receive
///     <see cref="RunLineType.Text"/>.
/// </param>
/// <param name="Text">
///     The unmodified raw text of the simulator output line, as captured from the combined
///     stdout and stderr stream.
/// </param>
public record RunLine(RunLineType Type, string Text);
